using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 3f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Height Settings")]
    public float standEyeHeight = 1.5f;  // 서 있을 때 눈높이
    public float crouchEyeHeight = 0.8f; // 웅크렸을 때 눈높이
    public float crouchTransitionSpeed = 8f; // 앉고 일어서는 속도

    private float currentEyeHeight; // 현재 실시간 눈높이

    [Header("Head Bobbing Settings")]
    public float walkBobSpeed = 12f;
    public float walkBobAmount = 0.05f;

    public float runBobSpeed = 16f;
    public float runBobAmount = 0.1f;

    public float crouchBobSpeed = 6f;
    public float crouchBobAmount = 0.02f;

    private float pitch = 0f;
    private float timer = 0f;
    private float defaultX;
    private float defaultZ;

    void Start()
    {
        defaultX = transform.localPosition.x;
        defaultZ = transform.localPosition.z;

        // 시작할 때는 서 있는 높이로 설정
        currentEyeHeight = standEyeHeight;
        transform.localPosition = new Vector3(defaultX, currentEyeHeight, defaultZ);
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandleCrouchHeight(); // 웅크리기 높이 계산 추가
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
        // PlayerMovement와 동일하게 LeftControl 키를 체크
        // Ctrl을 누르고 있으면 웅크린 높이, 떼면 서 있는 높이를 목표로 설정
        float targetHeight = Input.GetKey(KeyCode.LeftControl) ? crouchEyeHeight : standEyeHeight;

        // 목표 높이로 부드럽게 보간(Lerp)
        currentEyeHeight = Mathf.Lerp(currentEyeHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    void HandleHeadBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            float currentBobSpeed = walkBobSpeed;
            float currentBobAmount = walkBobAmount;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentBobSpeed = runBobSpeed;
                currentBobAmount = runBobAmount;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                currentBobSpeed = crouchBobSpeed;
                currentBobAmount = crouchBobAmount;
            }

            timer += Time.deltaTime * currentBobSpeed;
            // 기준점을 고정된 eyeHeight가 아니라 실시간으로 변하는 currentEyeHeight로 변경!
            float newY = currentEyeHeight + Mathf.Sin(timer) * currentBobAmount;

            transform.localPosition = new Vector3(defaultX, newY, defaultZ);
        }
        else
        {
            timer = 0f;
            // 멈췄을 때도 현재 눈높이(앉은 상태면 앉은 높이)로 부드럽게 복귀
            float resetY = Mathf.Lerp(transform.localPosition.y, currentEyeHeight, Time.deltaTime * 5f);
            transform.localPosition = new Vector3(defaultX, resetY, defaultZ);
        }
    }
}