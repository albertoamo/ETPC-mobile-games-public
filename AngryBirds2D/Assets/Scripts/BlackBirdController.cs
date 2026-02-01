using System.Collections;
using UnityEngine;

public class BlackBirdController : BirdController
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2.5f;      // Radio máximo de la explosión
    public float baseDamage = 150f;           // Daño máximo en el centro
    public float explosionForce = 400f;       // Fuerza máxima de empuje

    [Header("Effects")]
    public Transform explosionEffectPrefab;   // Partículas (opcional)
    public AudioClip explosionSound;          // Sonido (opcional)

    private bool hasExploded = false;
    private bool collisionScheduled = false;
    private AudioSource audioSource;

    void Start()
    {
        // Inicializa Rbody y lógica base del pájaro
        Initialize();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Si no está en vuelo o ya explotó, no hacemos nada
        if (!isActive || hasExploded)
            return;

        // Comportamiento base (como los otros pájaros)
        DetectAlive();
        DrawTrace();

        // 💣 Toque en cualquier parte de la pantalla mientras va volando
        if (Input.GetMouseButtonDown(0))
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si ya explotó o no está en vuelo, nada
        if (!isActive || hasExploded || collisionScheduled)
            return;

        // Programa explosión con pequeño delay tras el primer choque
        collisionScheduled = true;
        StartCoroutine(ExplosionAfterDelay());
    }

    private IEnumerator ExplosionAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.0f); // 1 seg después del impacto
        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Partículas
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Sonido
        if (explosionSound != null)
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.PlayOneShot(explosionSound);
        }

        // ☄️ Daño + fuerza en área (capas "Box" y "Enemy")
        int layerMask = LayerMask.GetMask("Box", "Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, layerMask);

        foreach (Collider2D col in hits)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            float t = Mathf.Clamp01(1f - dist / explosionRadius); // 1 en el centro, 0 en el borde

            // 1) Fuerza física
            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                rb.AddForce(dir * explosionForce * t, ForceMode2D.Impulse);
            }

            // 2) Daño si tiene HealthController
            HealthController health = col.GetComponent<HealthController>();
            if (health != null)
            {
                float damage = baseDamage * t;
                health.UpdateHealth(damage);
            }
        }

        // 🔻 A PARTIR DE AQUÍ: APAGAR Y DESTRUIR EL PÁJARO 🔻

        // Desactivar TODOS los SpriteRenderer del pájaro (por si hay hijos)
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            sr.enabled = false;
        }

        // Desactivar TODOS los Collider2D del pájaro
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Parar la física del pájaro
        Rigidbody2D myRb = GetComponent<Rigidbody2D>();
        if (myRb != null)
        {
            myRb.linearVelocity = Vector2.zero;
            myRb.angularVelocity = 0f;
            myRb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Este pájaro deja de estar activo
        isActive = false;

        // Pedimos el siguiente pájaro (usa la lógica que ya tienes montada)
        ReloadNext();

        // Destruimos el pájaro un poco después de la explosión
        Destroy(gameObject, 0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
