using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;
    public float rotationSensitivity = 3f; // 카메라 스크립트의 sensitivity와 맞추는 것이 좋음

    private CharacterController cc;
    private Vector3 velocity;
    private float yaw = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();

        // 커서 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 현재 플레이어의 초기 회전값(Yaw)을 가져와서 시작
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleGravity();
    }

    void HandleRotation()
    {
        // 마우스 좌우 입력으로 플레이어 몸통 자체를 회전시킴
        yaw += Input.GetAxis("Mouse X") * rotationSensitivity;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 카메라가 아닌 '플레이어 몸통'의 정면/오른쪽 방향을 기준으로 이동
        Vector3 moveDir = transform.right * x + transform.forward * z;

        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed = runSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) speed = crouchSpeed;

        // 이동 벡터를 정규화하여 대각선 이동 시 빨라지는 현상 방지
        if (moveDir.magnitude > 1) moveDir.Normalize();

        cc.Move(moveDir * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (cc.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}