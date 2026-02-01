using System.Collections;
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

    [Header("Distance")]
    public float distanceMeters = 0f;
    public float DistanceMeters => distanceMeters;



    // Lane change
    [HideInInspector] public int currentLane = 1;

    // Shield
    private bool _shieldActive = false;
    private Coroutine _shieldRoutine;
    private float _postHitInvuln = 0f;
    public bool ShieldActive => _shieldActive;

    private bool _isSliding = false;
    private bool _isAlive = true;
    private float _currentGravity = 0f;
    private Vector3 targetPosition;
    private CharacterController _charCtr;

    private float timeIncrement = 0f;
    private Vector3 _playerHorDir;

    void Start()
    {
        _charCtr = GetComponent<CharacterController>();
        targetPosition = transform.position;

        distanceMeters = 0f;   // ← AQUÍ
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) MoveLane(1);

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

        if (_postHitInvuln > 0f)
            _postHitInvuln -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ComputeGravity();

        timeIncrement += Time.fixedDeltaTime * speedIncremental;
        float forwardFinalSpeed = forwardSpeed + timeIncrement;
        distanceMeters += forwardFinalSpeed * Time.fixedDeltaTime;


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

        if (currentLane != newLane)
        {
            currentLane = newLane;
            float newx = (currentLane - 1) * laneDistance;
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

        if (Physics.CapsuleCast(p1, p2, _charCtr.radius * 0.5f, _playerHorDir, out hit, hitDistance, collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (_isAlive)
            {
                if (_postHitInvuln > 0f) return;

                if (_shieldActive)
                {
                    ConsumeShield();
                    return;
                }


                GameStateManager.Instance.ChangeGameState(GameState.StateType.OVER);
                _isAlive = false;
            }
        }

        if (Physics.CheckCapsule(p1, p2, _charCtr.radius, collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (_isAlive)
            {
                if (_postHitInvuln > 0f) return;

                if (_shieldActive)
                {
                    ConsumeShield();
                    return;
                }

                GameStateManager.Instance.ChangeGameState(GameState.StateType.OVER);
                _isAlive = false;
            }
        }
    }

    public void ActivateShield(float duration)
    {
        _shieldActive = true;

        if (_shieldRoutine != null) StopCoroutine(_shieldRoutine);
        _shieldRoutine = StartCoroutine(ShieldTimer(duration));
    }

    private IEnumerator ShieldTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        _shieldActive = false;
        _shieldRoutine = null;
    }

    private void ConsumeShield()
    {
        _shieldActive = false;
        _postHitInvuln = 0.5f;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!_shieldActive) return;

        int envLayer = LayerMask.NameToLayer("CollisionEnvironment");
        if (envLayer == -1) return;

        if (hit.gameObject.layer != envLayer) return;

        // 1) Consumir escudo (si quieres que sea 1 golpe)
        ConsumeShield();

        // 2) Encontrar el Tile (para NO destruirlo)
        var tile = hit.collider.GetComponentInParent<TileController>();

        // 3) Si hay tile, destruimos el primer hijo bajo el tile (el obstáculo), no el tile entero
        if (tile != null)
        {
            Transform obstacle = hit.collider.transform;

            // Subimos en la jerarquía hasta que el padre sea el Tile
            while (obstacle.parent != null && obstacle.parent != tile.transform)
            {
                obstacle = obstacle.parent;
            }

            Destroy(obstacle.gameObject);
            return;
        }

        // Si no pertenece a un tile, destruye solo el objeto del collider (sin root)
        Destroy(hit.collider.gameObject);
    }
    // test push
}