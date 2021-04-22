using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    #region Camera Movement Variables

    private const float MAXLookAngle = 50f;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Sprite crosshairImage;
    [SerializeField] private Color crosshairColor = Color.white;

    private Image _crosshairObject;
    private float _yaw;
    private float _pitch;

    #endregion

    #region Movement Variables

    private const float WalkSpeed = 5f;
    private const float MAXVelocityChange = 10f;

    private bool _isWalking;

    #region Sprint

    private const float SprintSpeed = 7f;
    private const float SprintFOV = 80f;
    private const float SprintFOVStepTime = 10f;

    private bool _isSprinting;

    #endregion

    #region Jump

    private const float JumpPower = 5f;

    private bool _isGrounded;

    #endregion

    #endregion

    #region Config User

    private float _fov = 60f;
    private float _mouseSensitivity = 60f;
    private KeyCode _sprintKey = KeyCode.LeftShift;
    private KeyCode _jumpKey = KeyCode.Space;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = _fov;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _crosshairObject.sprite = crosshairImage;
        _crosshairObject.color = crosshairColor;
    }

    private void Update()
    {
        #region Camera

        _yaw += _mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        _pitch -= _mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(_pitch, -MAXLookAngle, MAXLookAngle), _yaw, 0);
        transform.eulerAngles = new Vector3(0, _yaw, 0);

        #endregion

        #region Sprint

        if (_isSprinting)
        {
            playerCamera.fieldOfView =
                Mathf.Lerp(playerCamera.fieldOfView, SprintFOV, SprintFOVStepTime * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView =
                Mathf.Lerp(playerCamera.fieldOfView, _fov, SprintFOVStepTime * Time.deltaTime);
        }


        FireLightScript.Instance.SetRange(_isSprinting);

        #endregion

        #region Jump

        if (Input.GetKeyDown(_jumpKey) && _isGrounded) Jump();

        #endregion

        CheckGround();
    }

    private void FixedUpdate()
    {
        #region Movement

        var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _isWalking = (targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded);

        if (Input.GetKey(_sprintKey))
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * SprintSpeed;

            var velocity = _rb.velocity;
            var velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.y = 0;

            if (_isWalking && velocityChange.x != 0 || velocityChange.z != 0)
            {
                _isSprinting = true;
            }

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            _isSprinting = false;

            targetVelocity = transform.TransformDirection(targetVelocity) * WalkSpeed;

            var velocity = _rb.velocity;
            var velocityChange = (targetVelocity - velocity);
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

        if (Physics.Raycast(position, direction, out _, distance))
        {
            Debug.DrawRay(position, direction * distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void Jump()
    {
        if (!_isGrounded) return;
        _rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);
        _isGrounded = false;
    }
}