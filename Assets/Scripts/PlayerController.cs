using UnityEngine;
using static GameManager.GameState;

public class PlayerController : Singleton<PlayerController>
{
    private EnemyFollow[] _cannibals;

    #region Camera Movement

    private const float MAXLookAngle = 50f;

    [SerializeField] private Camera playerCamera;

    private const float SprintFOVStepTime = 10f;
    private const float FOV = 60f;
    private const float SprintFOV = 80f;
    private float _yaw;
    private float _pitch;

    #endregion

    #region Movement

    private Rigidbody _rb;
    private const float WalkSpeed = 250f;
    private const float MAXVelocityChange = 450f;

    [HideInInspector] public bool isWalking;
    
    private const float MouseSensitivity = 60f;
    private const KeyCode SprintKey = KeyCode.LeftShift;
    private const KeyCode JumpKey = KeyCode.Space;

    #region Sprint

    private const float SprintSpeed = 500f;

    [HideInInspector] public bool isSprinting;

    #endregion

    #region Jump

    private const float JumpPower = 5f;

    private bool _isGrounded;

    #endregion

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
        Cursor.lockState = GameManager.Instance.gameState == Playing ? CursorLockMode.Locked : CursorLockMode.None;
        
        if (GameManager.Instance.gameState != Playing) return;
        
        #region Camera

        _yaw += MouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        _pitch -= MouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(_pitch, -MAXLookAngle, MAXLookAngle), _yaw);
        transform.eulerAngles = Vector3.up * _yaw;

        #endregion

        #region Sprint

        var fovResult = isSprinting ? SprintFOV : FOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fovResult, SprintFOVStepTime * Time.deltaTime);

        TorchScript.Instance.SetRange(isSprinting);

        #endregion

        #region Jump

        if (Input.GetKeyDown(JumpKey) && _isGrounded) Jump();
        CheckGround();

        #endregion
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState != Playing) return;

        #region Movement

        var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        isWalking = targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded;
        Vector3 velocityChange;
        if (targetVelocity.z > 0 && Input.GetKey(SprintKey))
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * (SprintSpeed * Time.deltaTime);

            velocityChange = GetVelocityChange(targetVelocity - _rb.velocity);

            if (isWalking && velocityChange.x != 0 || velocityChange.z != 0)
                isSprinting = true;
        }
        else
        {
            isSprinting = false;

            targetVelocity = transform.TransformDirection(targetVelocity) * (WalkSpeed * Time.deltaTime);

            velocityChange = GetVelocityChange(targetVelocity - _rb.velocity);
        }
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);

        #endregion
    }

    private Vector3 GetVelocityChange(Vector3 vChange)
    {
        vChange.x = Mathf.Clamp(vChange.x, -MAXVelocityChange, MAXVelocityChange);
        vChange.z = Mathf.Clamp(vChange.z, -MAXVelocityChange, MAXVelocityChange);
        vChange.y = 0;
        return vChange;
    }

    private void CheckGround()
    {
        const float distance = 1.25f;
        var position = transform.position;
        var direction = transform.TransformDirection(Vector3.down);

        _isGrounded = Physics.Raycast(position, direction, out _, distance);
    }

    private void Jump()
    {
        if (!_isGrounded) return;
        _rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);
        _isGrounded = false;
    }
}