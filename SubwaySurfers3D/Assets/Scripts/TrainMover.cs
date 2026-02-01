using DG.Tweening;
using UnityEngine;

public class TrainMover : MonoBehaviour
{
    private Tween moveTween;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Evita crear el tween varias veces
            if (moveTween != null && moveTween.IsActive()) return;

            moveTween = transform.DOLocalMoveZ(-64, 7);
        }
    }

    private void OnDestroy()
    {
        // MUY IMPORTANTE: matar el tween antes de destruir
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill(false);
        }
    }
}
