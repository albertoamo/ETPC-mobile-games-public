using System;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    protected bool isActive = false;
    public float trailDelay = 0.3f;
    public Transform trailSprite;

   public Rigidbody2D Rbody;

    private Vector3 _timePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Initialize()
    {
        Rbody = GetComponent<Rigidbody2D>();
        _timePosition = transform.position;
    }
     
    protected void DetectAlive()
    {
        if (Rbody.linearVelocity.magnitude < 0.4f)
        {
            isActive = false;

            SlingshotController.instance.Reload();
        }
    }

    public void ReloadNext()
    {
        SlingshotController.instance.Reload();
    }

    public void SetBirdActive(bool activate)
    {
        isActive = activate;
    }

    public void DrawTrace()
    {
        if (isActive)
        {
            float dist = Vector2.Distance(_timePosition, transform.position);

            if(dist > 0.4)
            {
                Transform trail = Instantiate(trailSprite, transform.position, Quaternion.identity);
                trail.localScale = UnityEngine.Random.Range(0.5f, 1.2f) * Vector3.one;
                
                _timePosition = transform.position;
            }
        }
    }
}
