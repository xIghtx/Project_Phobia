using UnityEngine;
using System.Collections;

public class LoopManager : MonoBehaviour
{
    [Header("플레이어 컴포넌트 연결")]
    [SerializeField] private PlayerMovement player;

    [Header("천장 전구 오브젝트들 (5개 배치)")]
    [SerializeField] private GameObject[] lightBulbs;

    [Header("오디오 설정")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip glassBreakSound;    // 정답 시 전구 깨지는 소리
    [SerializeField] private AudioClip electricBuzzSound;  // 오답 시 지지직 소리

    private int correctCount = 0;
    private bool isFlashing = false;

    private void Start()
    {
        // 만약 인스펙터에서 플레이어를 안 넣었다면 자동으로 찾기
        if (player == null) player = FindAnyObjectByType<PlayerMovement>();

        // 오디오소스가 없으면 본인 오브젝트에서 가져오기
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // [정답 트리거] 플레이어가 올바른 복도를 통과했을 때 호출
    public void OnCorrectPath()
    {
        // 열쇠를 집기 전(폐소공포증 작동 전)에는 루프 기믹이 안 돌아가게 방어
        if (player == null || !player.isClaustrophobia) return;

        if (correctCount < lightBulbs.Length)
        {
            // 1. 전구 깨지는 소리 1회 재생
            if (audioSource && glassBreakSound)
                audioSource.PlayOneShot(glassBreakSound);

            // 2. 해당 순서의 전구 오브젝트 꺼버리기 (배열 순서대로)
            if (lightBulbs[correctCount] != null)
                lightBulbs[correctCount].SetActive(false);

            correctCount++;
            Debug.Log($"[IngMeme] 정답 복도 통과! 현재 파괴된 전구: {correctCount}/5");

            // 전구 5개가 모두 깨지면 (5바퀴 정답이면) 최종 탈출문 개방
            if (correctCount >= lightBulbs.Length)
            {
                OpenEscapeRoute();
            }
        }
    }

    // [오답 트리거] 플레이어가 잘못된 복도를 통과했을 때 호출
    public void OnWrongPath()
    {
        if (player == null || !player.isClaustrophobia) return;

        Debug.LogWarning("[IngMeme] 오답 복도 진입! 전구 과부하 발동.");

        // 오답 패널티: 리셋은 안 하되, 남은 전구들을 지지직거리며 깜빡이게 만듦
        if (!isFlashing)
        {
            StartCoroutine(FlashLightsCoroutine());
        }

        // 플레이어 패닉 가속 (스태미나를 15만큼 강제로 깎아버림)
        player.currentStamina -= 15f;
    }

    // 전구 지지직 연출 코루틴
    private IEnumerator FlashLightsCoroutine()
    {
        isFlashing = true;

        // 지지직 사운드 재생
        if (audioSource && electricBuzzSound)
        {
            audioSource.clip = electricBuzzSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 2초 동안 불규칙하게 전구를 껐다 켰다 반복 (착시 및 패닉 유도)
        for (int i = 0; i < 6; i++)
        {
            SetRemainingLights(false);
            yield return new WaitForSeconds(0.1f);
            SetRemainingLights(true);
            yield return new WaitForSeconds(0.15f);
        }

        // 사운드 정지
        if (audioSource) audioSource.Stop();
        isFlashing = false;
    }

    // 아직 파괴되지 않고 살아있는 전구들만 일괄 제어하는 함수
    private void SetRemainingLights(bool state)
    {
        for (int i = correctCount; i < lightBulbs.Length; i++)
        {
            if (lightBulbs[i] != null)
                lightBulbs[i].SetActive(state);
        }
    }

    private void OpenEscapeRoute()
    {
        Debug.Log("[IngMeme] 탈출 프로토콜: 중앙 방의 잠긴 문이 열렸습니다. 그리로 가세요!");
        // 여기에 2학기용 개구멍 활성화나 문 오픈 애니메이션 코드를 연동하면 됩니다.
    }
}