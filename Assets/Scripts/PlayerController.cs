using System;
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
    private const float SprintDuration = 5f;
    private const float SprintFOV = 80f;
    private const float SprintFOVStepTime = 10f;

    private float _sprintCooldown = .5f;
    private float _sprintRemaining;
    private float _sprintCooldownReset;
    private bool _isSprinting;
    private bool _isSprintCooldown;

    #region SprintBar

    private const float SprintBarWidthPercent = .3f;
    private const float SprintBarHeightPercent = .015f;

    [SerializeField] private Image sprintBar;

    [SerializeField] private CanvasGroup _sprintBarCg;

    #endregion

    #endregion

    #region Jump

    private const float JumpPower = 5f;

    private bool _isGrounded;

    #endregion

    #endregion

    #region Config User

    private float _fov = 60f;
    private float _mouseSensitivity = 2f;
    private KeyCode _sprintKey = KeyCode.LeftShift;
    private KeyCode _jumpKey = KeyCode.Space;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = _fov;

        _sprintRemaining = SprintDuration;
        _sprintCooldownReset = _sprintCooldown;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _crosshairObject.sprite = crosshairImage;
        _crosshairObject.color = crosshairColor;


        #region Sprint Bar

        _sprintBarCg = GetComponentInChildren<CanvasGroup>();

        sprintBar.gameObject.SetActive(true);

        var sprintBarWidth = Screen.width * SprintBarWidthPercent;
        var sprintBarHeight = Screen.height * SprintBarHeightPercent;

        sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

        #endregion
    }

    private void Update()
    {
        #region Camera

        _yaw += _mouseSensitivity * Input.GetAxis("Mouse X");
        _pitch -= _mouseSensitivity * Input.GetAxis("Mouse Y");

        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(_pitch, -MAXLookAngle, MAXLookAngle), _yaw, 0);
        transform.eulerAngles = new Vector3(0, _yaw, 0);

        #endregion

        #region Sprint

        if (_isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, SprintFOV, SprintFOVStepTime * Time.deltaTime);
            _sprintRemaining -= 1 * Time.deltaTime;
            if (_sprintRemaining <= 0)
            {
                _isSprinting = false;
                _isSprintCooldown = true;
            }
        }
        else
        {
            _sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, SprintDuration);
        }

        if (_isSprintCooldown)
        {
            _sprintCooldown -= 1 * Time.deltaTime;
            if (_sprintCooldown <= 0)
            {
                _isSprintCooldown = false;
            }
        }
        else
        {
            _sprintCooldown = _sprintCooldownReset;
        }


        var sprintRemainingPercent = _sprintRemaining / SprintDuration;
        sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);

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

        if (Input.GetKey(_sprintKey) && _sprintRemaining > 0f && !_isSprintCooldown)
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * SprintSpeed;

            var velocity = _rb.velocity;
            var velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -MAXVelocityChange, MAXVelocityChange);
            velocityChange.y = 0;

            if (velocityChange.x != 0 || velocityChange.z != 0)
            {
                _isSprinting = true;

                _sprintBarCg.alpha += 5 * Time.deltaTime;
            }

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            _isSprinting = false;

            if (Math.Abs(_sprintRemaining - SprintDuration) == 0)
            {
                _sprintBarCg.alpha -= 3 * Time.deltaTime;
            }

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
        const float distance = 1f;
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