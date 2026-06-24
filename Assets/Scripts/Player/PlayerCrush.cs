using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어 오브젝트에 부착하세요.
// CharacterController 또는 CapsuleCollider/BoxCollider 중 하나를 자동으로 인식해서
// 플레이어의 "너비"를 가져옵니다.
public class PlayerCrush : MonoBehaviour
{
    [Header("압사 판정을 검사할 ShrinkingWalls")]
    // 세 갈래 길 함정에 붙어있는 ShrinkingWalls 스크립트를 여기에 연결하세요.
    public ShrinkingWalls targetWalls;

    [Header("압사로 판정되는 비율 (0.75 = 플레이어 너비의 75%까지 남으면 압사)")]
    public float crushThresholdRatio = 0.75f;

    [Header("깜빡임 효과")]
    public float flashDuration = 3f;           // 전체 깜빡임 지속 시간
    public float startInterval = 0.3f;         // 처음 깜빡임 간격(느림)
    public float endInterval = 0.04f;          // 마지막 깜빡임 간격(빠름)

    private bool isCrushed = false;
    private float playerWidth = -1f;

    // 다른 스크립트(예: EscapeValve)에서 깜빡임 코루틴이 끝났을 때 처리할 동작을 바꿀 수 있게
    // 분리해둠. 지금은 SceneManager.LoadScene으로 현재 씬을 재시작.
    private System.Action onResetAction;

    void Start()
    {
        CalculatePlayerWidth();

        // [통합 예정] 나중에 EscapeValve의 루프 로직과 합칠 때
        // 아래 onResetAction에 EscapeValve가 쓰는 리셋 함수를 대신 넣어주면 됩니다.
        onResetAction = () =>
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        };
    }

    void CalculatePlayerWidth()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            playerWidth = cc.radius * 2f;
            return;
        }

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            playerWidth = capsule.radius * 2f;
            return;
        }

        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            // X, Z 중 더 큰 값을 너비로 사용
            playerWidth = Mathf.Max(box.size.x, box.size.z);
            return;
        }

        // 아무것도 못 찾으면 기본값으로 경고만 남기고 진행
        Debug.LogWarning("[PlayerCrush] CharacterController/Collider를 찾지 못해 기본 너비(0.5)를 사용합니다.");
        playerWidth = 0.5f;
    }

    void Update()
    {
        if (isCrushed) return;
        if (targetWalls == null || !targetWalls.isShrinking) return;
        if (playerWidth < 0f) return;

        float remainingWidth = GetRemainingCorridorWidth();
        float crushThreshold = playerWidth * crushThresholdRatio;

        if (remainingWidth <= crushThreshold)
        {
            TriggerCrush();
        }
    }

    // ShrinkingWalls가 누적시킨 localScale 증가량을 바탕으로 남은 통로 너비를 계산
    float GetRemainingCorridorWidth()
    {
        float totalShrinkAmount = 0f;

        if (targetWalls.leftWallPivot != null)
        {
            // X축 스케일 증가량만큼 벽이 두꺼워진 것 = 그만큼 통로가 줄어든 것
            totalShrinkAmount += (targetWalls.leftWallPivot.localScale.x - 1f);
        }

        if (targetWalls.rightWallPivot != null)
        {
            totalShrinkAmount += (targetWalls.rightWallPivot.localScale.x - 1f);
        }

        // 중복 입력 방지: ShrinkingWalls가 가진 corridorWidth를 그대로 참조.
        return targetWalls.corridorWidth - totalShrinkAmount;
    }

    void TriggerCrush()
    {
        isCrushed = true;
        CrushScreenFlash.Instance.PlayFlash(flashDuration, startInterval, endInterval, onResetAction);
    }
}