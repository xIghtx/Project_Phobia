using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 3f;
    public float minPitch = -85f;
    public float maxPitch = 85f;
    public float eyeHeight = 1.5f; // 캐릭터 발바닥 기준 눈높이 (보통 1.5 ~ 1.7)

    [Header("Head Bobbing Settings")]
    public float walkBobSpeed = 12f;
    public float walkBobAmount = 0.05f;

    public float runBobSpeed = 16f;
    public float runBobAmount = 0.1f;

    public float crouchBobSpeed = 6f;
    public float crouchBobAmount = 0.02f;

    private float pitch = 0f;
    private float timer = 0f;

    // 원래 카메라의 앞뒤(Z)/좌우(X) 위치를 저장할 변수
    private float defaultX;
    private float defaultZ;

    void Start()
    {
        // 에디터에서 유저가 직접 세팅한 카메라의 초기 X, Z 위치를 기억해둠!
        defaultX = transform.localPosition.x;
        defaultZ = transform.localPosition.z;

        // 시작할 때 Y값만 눈 높이로 초기화
        transform.localPosition = new Vector3(defaultX, eyeHeight, defaultZ);
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleHeadBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 입력이 있어서 이동 중일 때
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
            float newY = eyeHeight + Mathf.Sin(timer) * currentBobAmount;

            // X와 Z는 처음 설정한 값을 유지하고, Y만 위아래로 흔들리게 수정!
            transform.localPosition = new Vector3(defaultX, newY, defaultZ);
        }
        else
        {
            timer = 0f;
            float resetY = Mathf.Lerp(transform.localPosition.y, eyeHeight, Time.deltaTime * 5f);

            // X와 Z는 유지!
            transform.localPosition = new Vector3(defaultX, resetY, defaultZ);
        }
    }
}