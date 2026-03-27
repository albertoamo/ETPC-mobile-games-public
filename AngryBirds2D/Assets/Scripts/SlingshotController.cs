using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    public static SlingshotController instance;

    [SerializeField] private BirdController _currentBird;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private float _force = 350f;
    [SerializeField] private float _maxDistance = 3f;
    [SerializeField] public Transform _currentTarget;

    [SerializeField] public LineRenderer _lineFront;
    [SerializeField] public LineRenderer _lineBack;

    private Camera _camera;
    private bool _isDragging;
    private Vector2 _startOrigin;
    private Vector2 _direction;
    private float _distance;

    public bool isActive = true;

    private float _timelerp = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
        _startOrigin = new Vector2(_startPosition.position.x, _startPosition.position.y);
        _currentTarget = _currentBird.transform;

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = false;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics2D.Raycast(ray.origin, ray.direction))
            {
                _isDragging = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;

            Shot();
        }

        OnDrag();
    }

    public void OnDrag()
    {
        if (_isDragging)
        {
            _timelerp = 0f;

            Vector2 position = _camera.ScreenToWorldPoint(Input.mousePosition);
            _direction = position - _startOrigin;

            if (_direction.magnitude > _maxDistance)
            {
                position = _startOrigin + _direction.normalized * _maxDistance;
            }

            _distance = (position - _startOrigin).magnitude;

            Vector3 ropePosition = position + _direction.normalized * 0.15f;
            Vector3 mappedLine = new Vector3(ropePosition.x, ropePosition.y, 0);
            _lineFront.SetPosition(0, mappedLine);
            _lineBack.SetPosition(0, mappedLine);

            _currentBird.transform.position = position;
        }
        else
        {
            _timelerp += Time.deltaTime;

            Vector3 pos1 = Vector3.MoveTowards(_lineFront.GetPosition(0), _lineFront.GetPosition(1), _timelerp);
            Vector3 pos2 = Vector3.MoveTowards(_lineBack.GetPosition(0), _lineBack.GetPosition(1), _timelerp);
            
            _lineFront.SetPosition(0, pos1);
            _lineBack.SetPosition(0, pos2);
        }
    }

    public void Shot()
    {
        _currentBird.Rbody.bodyType = RigidbodyType2D.Dynamic;
        
        float forceImpulse = _distance / _maxDistance;
        Vector2 direction = _startPosition.position - _currentBird.transform.position;
        _currentBird.Rbody.AddForce(direction.normalized * _force * forceImpulse);
      
        Debug.Log(_distance + " " + _maxDistance + " -- " + forceImpulse);

        Invoke(nameof(ActivateBird), 0.1f);
    }
    public void Reload()
    {
        // We have to reload the new bird from the ammocontroller
        _currentBird = AmmoController.instance.Reload();

        if (_currentBird != null)
        {
            _currentTarget = _currentBird.transform;
            _currentBird.transform.position = _startPosition.position;

            // We need to move the camera to its original position
            CameraController.instance.ResetCamera();
        }
        else
        {
            // The game is over.
        }
    }

    public void ActivateBird()
    {
        BirdController bird = _currentBird.GetComponent<BirdController>();
        bird.SetBirdActive(true);
    }

    public Transform GetCurrentBird()
    {
        return _currentBird.transform;
    }

    public Transform GetCurrentTarget()
    {
        return _currentTarget;
    }

    public void SetCurrentTarget(Transform target)
    {
        _currentTarget = target;
    }
}
