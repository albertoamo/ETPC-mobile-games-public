using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform render;
    public Animator animator;

    public float forwardSpeed = 10f;
    public float verticalSpeed = 20f;
    public float laneSwapSpeed = 5f;
    public float laneDistance = 4f;

    public float jumpHeight = 1f;
    public float gravity = -9.81f;

    public float slideHeight = 0.5f;
    public float slideTime = 1f;

    public float hitDistance = 0.1f;
    public LayerMask collisionLayerMask;
    public float speedIncremental = 0.01f;

    // Lane change
    [HideInInspector] public int currentLane = 1;

    private bool _isSliding = false;
    private bool _isAlive = true;
    private float _currentGravity = 0f;
    private Vector3 targetPosition;
    private CharacterController _charCtr;

    private float timeIncrement = 0f;
    private Vector3 _playerHorDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _charCtr = GetComponent<CharacterController>();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLane(-1);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            MoveLane(1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && _charCtr.isGrounded)
        {
            _currentGravity = jumpHeight;
            animator.SetBool("Jump", true);
        }

        if (Input.GetKeyDown(KeyCode.S) && !_isSliding)
        {
            StartCoroutine(Slide());
        }

        CheckHealth();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        ComputeGravity();

        timeIncrement += Time.fixedDeltaTime * speedIncremental;
        float forwardFinalSpeed = forwardSpeed + timeIncrement;

        Vector3 forwardMove = Vector3.forward * forwardFinalSpeed * Time.fixedDeltaTime;
        Vector3 verticalMove = Vector3.up * _currentGravity;
        Vector3 horizontalMove = Vector3.MoveTowards(_charCtr.transform.position, targetPosition, laneSwapSpeed * Time.fixedDeltaTime);
        horizontalMove = new Vector3(horizontalMove.x - transform.position.x, 0, 0);
        _playerHorDir = forwardMove + horizontalMove;

        _charCtr.Move(_playerHorDir + verticalMove);
    }

    private void MoveLane(int direction)
    {
        int newLane = currentLane + direction;
        newLane = Mathf.Clamp(newLane, 0, 2);

        if(currentLane != newLane)
        {
            currentLane = newLane;
            float newx = (currentLane -1) * laneDistance;
            targetPosition = new Vector3(newx, transform.position.y, transform.position.z);
        }
    }

    public void ComputeGravity()
    {
        if (_charCtr.isGrounded && _currentGravity < 0)
        {
            _currentGravity = -0.5f;
            animator.SetBool("Jump", false);
        }

        _currentGravity += gravity * Time.fixedDeltaTime;
    }

    public IEnumerator Slide()
    {
        _isSliding = true;

        // Set collider and visuals to sliding
        Vector3 oldCenter = _charCtr.center;
        float oldHeight = _charCtr.height;

        _charCtr.center = Vector3.up * slideHeight;
        _charCtr.height = oldHeight * slideHeight;

        render.localScale = new Vector3(0.7f, 0.4f, 0.7f);
        render.transform.localPosition = Vector3.up * 0.4f;

        animator.SetBool("Roll", true);
        yield return new WaitForEndOfFrame();

        animator.SetBool("Roll", false);

        yield return new WaitForSeconds(slideTime);

        // Reset to default values
        _charCtr.center = oldCenter;
        _charCtr.height = oldHeight;

        render.localScale = Vector3.one;
        render.transform.localPosition = Vector3.up;

        _isSliding = false;
    }

    public void CheckHealth()
    {   
        RaycastHit hit;
        Vector3 p1 = transform.position + _playerHorDir * _charCtr.radius;
        Vector3 p2 = p1 + Vector3.up * _charCtr.height * 0.5f;

        if(Physics.CapsuleCast(p1, p2, _charCtr.radius * 0.5f, _playerHorDir, out hit, hitDistance, collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(hit.collider.name, hit.collider.gameObject);
            if(_isAlive)
            {
                GameStateManager.Instance.ChangeGameState(GameState.StateType.OVER);
                _isAlive = false;
            }
        }

        if (Physics.CheckCapsule(p1, p2, _charCtr.radius, collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (_isAlive)
            {
                GameStateManager.Instance.ChangeGameState(GameState.StateType.OVER);
                _isAlive = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 p1 = transform.position; 
        Vector3 p2 = p1 + Vector3.up * _charCtr.height * 0.75f;

        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        //Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(p1, Vector3.one * 0.25f);
        Gizmos.DrawCube(p2, Vector3.one * 0.25f);
    }
}
