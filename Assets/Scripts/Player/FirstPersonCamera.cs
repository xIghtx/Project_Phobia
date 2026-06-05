using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 3f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Height Settings")]
    public float standEyeHeight = 1.5f;
    public float crouchEyeHeight = 0.8f;
    public float crouchTransitionSpeed = 8f;

    private float currentEyeHeight;

    [Header("Head Bobbing Settings")]
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.015f;

    public float walkBobSpeed = 12f;
    public float walkBobAmount = 0.05f;

    public float runBobSpeed = 16f;
    public float runBobAmount = 0.1f;

    public float crouchBobSpeed = 6f;
    public float crouchBobAmount = 0.02f;

    [Header("Panic Settings (공포 상태)")]
    public float panicSpeedMultiplier = 1.5f;
    public float panicAmountMultiplier = 1.2f;

    private float pitch = 0f;
    private float timer = 0f;
    private float defaultX;
    private float defaultZ;

    private PlayerMovement player;

    void Start()
    {
        defaultX = transform.localPosition.x;
        defaultZ = transform.localPosition.z;

        currentEyeHeight = standEyeHeight;
        transform.localPosition = new Vector3(defaultX, currentEyeHeight, defaultZ);

        player = GetComponentInParent<PlayerMovement>();
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandleCrouchHeight();
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleCrouchHeight()
    {
        bool isCrouchingOrSliding = Input.GetKey(KeyCode.LeftControl) || (player != null && player.isSliding);
        float targetHeight = isCrouchingOrSliding ? crouchEyeHeight : standEyeHeight;

        currentEyeHeight = Mathf.Lerp(currentEyeHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    void HandleHeadBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        bool isSliding = player != null && player.isSliding;
        bool isPanic = player != null && player.isClaustrophobia;
        // [수정 포인트] Shift 키를 눌렀는지가 아니라, 플레이어가 "진짜 달리고 있는지"를 확인!
        bool isActuallyRunning = player != null && player.isActuallyRunning;

        float currentSpeedMult = isPanic ? panicSpeedMultiplier : 1f;
        float currentAmountMult = isPanic ? panicAmountMultiplier : 1f;

        if (isSliding)
        {
            timer = 0f;
            float resetY = Mathf.Lerp(transform.localPosition.y, currentEyeHeight, Time.deltaTime * 5f);
            transform.localPosition = new Vector3(defaultX, resetY, defaultZ);
        }
        else if (isMoving)
        {
            float currentBobSpeed = walkBobSpeed;
            float currentBobAmount = walkBobAmount;

            if (isActuallyRunning)
            {
                currentBobSpeed = runBobSpeed;
                currentBobAmount = runBobAmount;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                currentBobSpeed = crouchBobSpeed;
                currentBobAmount = crouchBobAmount;
            }

            timer += Time.deltaTime * (currentBobSpeed * currentSpeedMult);
            float newY = currentEyeHeight + Mathf.Sin(timer) * (currentBobAmount * currentAmountMult);

            transform.localPosition = new Vector3(defaultX, newY, defaultZ);
        }
        else
        {
            timer += Time.deltaTime * (idleBobSpeed * currentSpeedMult);
            float targetY = currentEyeHeight + Mathf.Sin(timer) * (idleBobAmount * currentAmountMult);

            float smoothY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * 5f);
            transform.localPosition = new Vector3(defaultX, smoothY, defaultZ);
        }
    }
}