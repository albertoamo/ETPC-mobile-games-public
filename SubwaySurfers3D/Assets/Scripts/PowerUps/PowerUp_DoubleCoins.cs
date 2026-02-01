using UnityEngine;

public class PowerUp_DoubleCoins : MonoBehaviour
{
    public float duration = 6f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CoinWallet.Instance.ActivateDoubleCoins(duration);
        Destroy(gameObject);
    }
}
