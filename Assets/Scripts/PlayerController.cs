using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager.GameState;

public class PlayerController : Singleton<PlayerController>
{
    private Rigidbody _rb;
    private EnemyFollow[] _cannibals;
    
    #region Camera Movement Variables

    private const float MAXLookAngle = 50f;

    [SerializeField] private Camera playerCamera;

    private float _yaw;
    private float _pitch;

    #endregion

    #region Movement Variables

    private const float WalkSpeed = 250f;
    private const float MAXVelocityChange = 450f;

    [HideInInspector] public bool isWalking;

    #region Sprint

    private const float SprintSpeed = 500f;
    private const float SprintFOV = 80f;
    private const float SprintFOVStepTime = 10f;

    [HideInInspector] public bool isSprinting;

    #endregion

    #region Jump

    private const float JumpPower = 5f;

    private bool _isGrounded;

    #endregion

    #endregion

    #region Config User

    private const float FOV = 60f;
    private const float MouseSensitivity = 60f;
    private const KeyCode SprintKey = KeyCode.LeftShift;
    private const KeyCode JumpKey = KeyCode.Space;

    #endregion
    

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();

        playerCamera.fieldOfView = FOV;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (GameManager.Instance.gameState!=Playing) return;

        #region Camera

        _yaw += MouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        _pitch -= MouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(_pitch, -MAXLookAngle, MAXLookAngle), _yaw, 0);
        transform.eulerAngles = new Vector3(0, _yaw, 0);

        #endregion

        #region Sprint

        var fovResult = isSprinting ? SprintFOV : FOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fovResult, SprintFOVStepTime * Time.deltaTime);

        if (TorchScript.IsInitialized) TorchScript.Instance.SetRange(isSprinting);

        #endregion

        #region Jump

        if (Input.GetKeyDown(JumpKey) && _isGrounded) Jump();
        CheckGround();

        #endregion
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState!=Playing) return;
        #region Movement

        var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        isWalking = targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded;

        if (targetVelocity.z > 0 && Input.GetKey(SprintKey))
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * (SprintSpeed * Time.deltaTime);

            var velocity = _rb.velocity;
            var velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.y = 0;

            if (isWalking && velocityChange.x != 0 || velocityChange.z != 0)
                isSprinting = true;

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            isSprinting = false;

            targetVelocity = transform.TransformDirection(targetVelocity) * (WalkSpeed * Time.deltaTime);

            var velocity = _rb.velocity;
            var velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.y = 0;

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        #endregion
    }

    private void CheckGround()
    {
        const float distance = 1.25f;
        var position = transform.position;
        var direction = transform.TransformDirection(Vector3.down);

        _isGrounded = Physics.Raycast(position, direction, out _, distance);

        if (_isGrounded)
            Debug.DrawRay(position, direction * distance, Color.red);
    }

    private void Jump()
    {
        if (!_isGrounded) return;
        _rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);
        _isGrounded = false;
    }
}