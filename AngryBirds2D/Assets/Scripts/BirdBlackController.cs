using System.Collections;
using UnityEngine;

public class BirdBlackController : BirdController
{
    public float explosionRadius = 2f;
    public float explosionForce = 500f;
    public LayerMask affectedLayers;

    private bool _exploded;

    void Start()
    {
        Initialize();
        _exploded = false;
    }

    void Update()
    {
        if (isActive)
        {
            // Si ya fue destruido, salir (seguridad extra)
            if (this == null) return;

            DetectAlive();
            DrawTrace();

            if (!_exploded && Input.GetKeyDown(KeyCode.Space))
            {
                Explode();
            }
        }
    }

    private void Explode()
    {
        Debug.Log("BOOM!");

        _exploded = true;

        // Detectar objetos cercanos
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, affectedLayers);

        foreach (Collider2D obj in objects)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                rb.AddForce(direction * explosionForce);
            }
        }

        // ✅ AVISAR al Slingshot antes de destruir
        if (SlingshotController.instance != null)
        {
            SlingshotController.instance.SetCurrentTarget(null);
        }

        // Destruir el pájaro
        Destroy(gameObject);
    }

    // Solo para ver el radio de explosión en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
