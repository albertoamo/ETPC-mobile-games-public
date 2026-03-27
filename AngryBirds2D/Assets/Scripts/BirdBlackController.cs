using System.Collections;
using UnityEngine;

public class BirdBlackController : BirdController
{
    public float RadioDeExplosion = 2.5f;
    public float FuerzaExplosiva = 350f;
    public float DañoDeExplosion = 40f;
    public LayerMask affectedLayers;

    private bool AExplotado = false;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        if (isActive)
        {
            DetectAlive();
            DrawTrace();

            if (!AExplotado && Input.GetKeyDown(KeyCode.Space))
            {
                TriggerExplosion();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!AExplotado && isActive)
        {
            TriggerExplosion();
        }
    }

    void TriggerExplosion()
    {
        AExplotado = true;

        ApplyExplosionDamage();
        ApplyExplosionPhysics();

        Rbody.linearVelocity = Vector2.zero;
        Rbody.bodyType = RigidbodyType2D.Static;

        StartCoroutine(DestroyAndReload());
    }

    IEnumerator DestroyAndReload()
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
        SlingshotController.instance.Reload();
    }

    void ApplyExplosionPhysics()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, RadioDeExplosion, affectedLayers);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb != null)
            {
                Vector2 dir = hit.transform.position - transform.position;
                float falloff = Mathf.Clamp01(1 - dir.magnitude / RadioDeExplosion);

                rb.AddForce(dir.normalized * FuerzaExplosiva * falloff, ForceMode2D.Impulse);
            }
        }
    }

    void ApplyExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, RadioDeExplosion, affectedLayers);

        foreach (Collider2D hit in hits)
        {
            HealthController health = hit.GetComponent<HealthController>();

            if (health != null)
            {
                float distance = Vector2.Distance(hit.transform.position, transform.position);
                float falloff = Mathf.Clamp01(1 - (distance / RadioDeExplosion));

                float finalDamage = FuerzaExplosiva * falloff;


                health.UpdateHealth(finalDamage);
            }
        }
    }


}