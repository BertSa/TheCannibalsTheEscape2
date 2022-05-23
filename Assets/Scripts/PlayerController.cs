using Enums;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private const float WalkSpeed = 250f;
    private const float SprintSpeed = 500f;
    private const float MaxVelocityChange = 450f;
    private const float JumpPower = 5f;
    private const KeyCode SprintKey = KeyCode.LeftShift;
    private const KeyCode JumpKey = KeyCode.Space;

    private Rigidbody Rb { get; set; }
    public bool IsWalking { get; private set; }
    public bool IsSprinting { get; private set; }
    private bool IsGrounded { get; set; }

    private bool IsPlaying => GameManager.Instance.State == GameState.Playing;

    protected override void Awake()
    {
        base.Awake();
        Rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        Jump();

        CheckGround();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!IsPlaying)
        {
            return;
        }

        var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        IsWalking = IsGrounded && targetVelocity.x != 0 || targetVelocity.z != 0;

        IsSprinting = targetVelocity.z > 0 && Input.GetKey(SprintKey);

        var velocityChange = GetVelocityChange(targetVelocity);

        Rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private Vector3 GetVelocityChange(Vector3 targetVelocity)
    {
        var speed = IsSprinting ? SprintSpeed : WalkSpeed;

        targetVelocity = transform.TransformDirection(targetVelocity) * (speed * Time.deltaTime);

        var velocityChange = targetVelocity - Rb.velocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
        velocityChange.y = 0;


        return velocityChange;
    }

    private void CheckGround()
    {
        const float distance = 1.25f;
        var position = transform.position;
        var direction = transform.TransformDirection(Vector3.down);

        IsGrounded = Physics.Raycast(position, direction, out _, distance);
    }

    private void Jump()
    {
        if (!IsGrounded)
        {
            return;
        }

        if (!Input.GetKeyDown(JumpKey))
        {
            return;
        }

        Rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);

        IsGrounded = false;
    }
}