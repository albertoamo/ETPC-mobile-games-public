using UnityEngine;

public class CollectableCoin : MonoBehaviour
{
    private BoxCollider _box;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _box = GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Play particle
            // Sum the coin to my wallet
            Destroy(gameObject);
        }
    }
}
