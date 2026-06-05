using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;
    public float rotationSensitivity = 3f;

    [Header("Sliding Settings")]
    public float slideDeceleration = 6f;
    public float crouchHeight = 1f;
    public float normalHeight = 2f;
    public bool isSliding = false;
    private float currentSlideSpeed;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float drainRate = 25f;
    public float regenRate = 15f;
    public float reRunThreshold = 20f;

    [Header("Stamina UI (ASCII)")]
    public TextMeshProUGUI staminaText;
    private Color normalColor;
    private Color exhaustedColor = Color.red;

    [Header("Claustrophobia")]
    public bool isClaustrophobia = false;
    public Camera mainCamera;
    public float defaultFOV = 60f;
    public float panicFOV = 35f;
    public float fovSpeed = 2f;

    private CharacterController cc;
    private Vector3 velocity;
    private float yaw = 0f;

    public bool isExhausted = false;
    public bool isActuallyRunning = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;
        currentStamina = maxStamina;

        if (mainCamera != null) mainCamera.fieldOfView = defaultFOV;
        ColorUtility.TryParseHtmlString("#FFE541", out normalColor);

        if (staminaText != null)
        {
            staminaText.alignment = TextAlignmentOptions.Center;
            UpdateStaminaUI();
        }
    }

    void Update()
    {
        if (cc == null || !cc.enabled || !gameObject.activeInHierarchy) return;

        HandleRotation();
        HandleMovementAndPosture();
        HandleGravity();
        HandleVignetteAndFOV();
        UpdateStaminaUI();
    }

    void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSensitivity;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void HandleMovementAndPosture()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDir = transform.right * x + transform.forward * z;
        if (moveDir.magnitude > 1) moveDir.Normalize();

        bool isMoving = (x != 0 || z != 0);
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);
        bool isCrouchInput = Input.GetKey(KeyCode.LeftControl);
        bool isCrouchDown = Input.GetKeyDown(KeyCode.LeftControl);

        isActuallyRunning = false;

        if (currentStamina <= 0f) isExhausted = true;
        if (isExhausted && !isRunningInput && currentStamina >= reRunThreshold) isExhausted = false;

        // =================================================================
        // 🚨 [핵심 추가] 천장 충돌 방지 센서 🚨
        // 유저가 Ctrl에서 손을 뗐지만(일어서려고 함), 현재 몸이 웅크린 상태라면?
        if (!isCrouchInput && cc.height < normalHeight - 0.1f)
        {
            // 플레이어의 정수리 위치를 계산
            Vector3 topOfPlayer = cc.bounds.center + Vector3.up * cc.bounds.extents.y;
            float spaceNeeded = (normalHeight - cc.height) + 0.1f;

            // 정수리 위로 보이지 않는 레이저를 쏴서 천장에 닿는지 확인!
            if (Physics.Raycast(topOfPlayer, Vector3.up, spaceNeeded))
            {
                // 공간이 없으면 유저가 키보드에서 손을 떼도 강제로 웅크리기 유지!
                isCrouchInput = true;
            }
        }
        // =================================================================

        float speed = walkSpeed;
        float targetHeight = normalHeight;

        if (isCrouchDown && isRunningInput && isMoving && !isExhausted && !isSliding)
        {
            isSliding = true;
            currentSlideSpeed = runSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSliding = false;
        }

        if (isSliding)
        {
            targetHeight = crouchHeight;
            currentSlideSpeed -= slideDeceleration * Time.deltaTime;

            if (currentSlideSpeed <= crouchSpeed)
            {
                isSliding = false;
                speed = crouchSpeed;
            }
            else speed = currentSlideSpeed;

            float currentDrain = isClaustrophobia ? drainRate * 2f : drainRate;
            currentStamina -= currentDrain * Time.deltaTime;
        }
        else if (isCrouchInput)
        {
            speed = crouchSpeed;
            targetHeight = crouchHeight;
            if (currentStamina < maxStamina) currentStamina += regenRate * Time.deltaTime;
        }
        else if (isRunningInput && isMoving && !isExhausted)
        {
            isActuallyRunning = true;
            speed = runSpeed;
            targetHeight = normalHeight;
            float currentDrain = isClaustrophobia ? drainRate * 2f : drainRate;
            currentStamina -= currentDrain * Time.deltaTime;
        }
        else
        {
            speed = walkSpeed;
            targetHeight = normalHeight;
            if (currentStamina < maxStamina) currentStamina += regenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        cc.height = Mathf.Lerp(cc.height, targetHeight, Time.deltaTime * 10f);
        cc.Move(moveDir * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    void HandleVignetteAndFOV()
    {
        if (mainCamera == null) return;
        float targetFOV = isClaustrophobia ? panicFOV : defaultFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }

    void UpdateStaminaUI()
    {
        if (staminaText != null)
        {
            int maxBlocks = isClaustrophobia ? 4 : 10;
            int activeBlocks = Mathf.RoundToInt((currentStamina / maxStamina) * maxBlocks);

            string barString = "[";
            for (int i = 0; i < maxBlocks; i++)
            {
                if (i < activeBlocks) barString += "■";
                else barString += "□";
            }
            barString += "]";

            staminaText.text = barString;
            staminaText.color = isExhausted ? exhaustedColor : normalColor;
        }
    }

    public void ActivateClaustrophobia()
    {
        isClaustrophobia = true;
    }
}