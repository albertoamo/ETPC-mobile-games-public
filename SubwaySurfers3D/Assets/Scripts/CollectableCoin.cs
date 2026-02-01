using UnityEngine;

public class CollectableCoin : MonoBehaviour
{
    private BoxCollider _box;

    void Start()
    {
        _box = GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play particle (si tienes)
            CoinWallet.Instance.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
