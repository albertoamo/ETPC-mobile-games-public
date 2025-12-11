using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public static AmmoController instance;

    public int maxAmmoCount = 5;
    public float offset = 0.05f;
    public Transform birdPrefab;

    public List<BirdController> _birds;

    private int _ammoCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;    
        _ammoCount = maxAmmoCount;

        float size = birdPrefab.GetComponent<CircleCollider2D>().radius * 2f + offset;

        for (int i = 0; i < maxAmmoCount - 1; i++)
        {
            Vector3 pos = transform.position + Vector3.left * i * size;
            Transform bird = Instantiate(birdPrefab, pos, Quaternion.identity);
            bird.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            BirdController bCtr = bird.GetComponent<BirdController>();
            _birds.Add(bCtr);

        }
        
        _birds.Reverse();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BirdController Reload()
    {
        _ammoCount = _ammoCount - 1;

        if (_ammoCount > 0)
        {
            return _birds[_ammoCount - 1];
        }
        else
        {
            GameStateManager.Instance.ChangeGameState(GameState.StateType.OVER);
        }

        // We should evaluate if we won.
        return null;
    }
}
