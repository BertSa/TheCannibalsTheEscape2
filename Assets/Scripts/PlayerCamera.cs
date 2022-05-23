using Enums;
using UnityEngine;

public class PlayerCamera : Singleton<PlayerCamera>
{
    private const float MAXLookAngle = 50f;
    private const float MouseSensitivity = 60f;
    private const float SprintFOVStepTime = 10f;
    private const float FOV = 60f;
    private const float SprintFOV = 80f;

    [SerializeField] private Camera playerCamera;

    private float Yaw { get; set; }
    private float Pitch { get; set; }

    private bool IsPlaying => GameManager.Instance.State == GameState.Playing;
    private bool IsSprinting => PlayerController.Instance.IsSprinting;

    protected override void Awake()
    {
        base.Awake();

        playerCamera.fieldOfView = FOV;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

        MoveCamera();

        SetFOV();
    }

    private void MoveCamera()
    {
        Yaw += MouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        Pitch -= MouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(Pitch, -MAXLookAngle, MAXLookAngle), Yaw);
        transform.eulerAngles = Vector3.up * Yaw;
    }

    private void SetFOV()
    {
        var fovResult = IsSprinting ? SprintFOV : FOV;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fovResult, SprintFOVStepTime * Time.deltaTime);
    }
}
