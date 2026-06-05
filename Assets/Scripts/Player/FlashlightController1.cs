using System.Collections;
using UnityEngine;

public class FlashlightController1 : MonoBehaviour
{
    [Header("Flashlight Setup")]
    public Light flashlight;

    [Header("Flicker Settings")]
    public int minFlickers = 2;       // 최소 깜빡임 횟수
    public int maxFlickers = 4;       // 최대 깜빡임 횟수
    public float minFlickerSpeed = 0.05f; // 가장 빠른 깜빡임 간격
    public float maxFlickerSpeed = 0.15f; // 가장 느린 깜빡임 간격

    private bool isLightOn = true;
    private bool isCrouching = false; // 현재 웅크리고 있는지 체크
    private Coroutine flickerCoroutine;

    // [추가] 플레이어의 실제 물리적 크기(높이)를 확인하기 위한 변수
    private CharacterController playerCC;

    void Start()
    {
        if (flashlight != null) flashlight.enabled = isLightOn;

        // 시작할 때 부모 오브젝트(Player)에 있는 CharacterController를 찾아옵니다.
        playerCC = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (flashlight == null) return;

        bool currentCrouch = false;

        // [핵심 해결] 키보드가 아니라 '실제 몸통의 높이'를 확인합니다!
        if (playerCC != null)
        {
            // 평소 높이(2.0)와 웅크린 높이(1.0)의 중간인 1.5를 기준으로 잡습니다.
            // 몸통 높이가 1.5 이하라면 확실하게 웅크리고 있는 상태!
            currentCrouch = playerCC.height <= 1.5f;
        }
        else
        {
            // 혹시라도 못 찾았다면 기존처럼 키보드 입력 사용
            currentCrouch = Input.GetKey(KeyCode.LeftControl);
        }

        // 상태가 변했을 때(진짜로 몸이 낮아지거나, 진짜로 몸이 커졌을 때)만 실행
        if (currentCrouch != isCrouching)
        {
            isCrouching = currentCrouch;

            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
            }

            // 웅크리면(!true = false) 꺼져야 하고, 일어서면(!false = true) 켜져야 함
            flickerCoroutine = StartCoroutine(FlickerRoutine(!isCrouching));
        }
    }

    IEnumerator FlickerRoutine(bool targetState)
    {
        int flickers = Random.Range(minFlickers, maxFlickers + 1);

        for (int i = 0; i < flickers; i++)
        {
            flashlight.enabled = !flashlight.enabled;
            yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
        }

        isLightOn = targetState;
        flashlight.enabled = isLightOn;
        flickerCoroutine = null;
    }
}