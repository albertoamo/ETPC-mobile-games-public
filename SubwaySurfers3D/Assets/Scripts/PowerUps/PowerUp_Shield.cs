using UnityEngine;

public class PowerUp_Shield : MonoBehaviour
{
    public float duration = 8f; // si quieres que expire con tiempo

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player) player.ActivateShield(duration);

        Destroy(gameObject);
    }
}