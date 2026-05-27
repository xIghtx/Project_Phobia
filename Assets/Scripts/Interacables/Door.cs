using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f; // 문이 열릴 각도
    public float smoothSpeed = 3f; // 문이 열리는 속도

    private bool isOpen = false;
    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // 시작할 때의 처음 회전값을 저장
        defaultRotation = transform.localRotation;
        targetRotation = defaultRotation;
    }

    void Update()
    {
        // 목표 회전값으로 부드럽게 회전 (Slerp)
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    // 상호작용했을 때 호출될 함수
    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            // 처음 회전값에서 y축으로 openAngle만큼 회전한 값을 목표로 설정
            targetRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);
        }
        else
        {
            // 다시 원래 닫힌 회전값으로 복귀
            targetRotation = defaultRotation;
        }
    }
}