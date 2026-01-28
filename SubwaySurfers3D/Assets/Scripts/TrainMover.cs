using DG.Tweening;
using UnityEngine;

public class TrainMover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            transform.DOLocalMoveZ(-64, 7);
        }
    }
}
