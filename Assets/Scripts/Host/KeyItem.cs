using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private StageManager stageManager;

    void Start()
    {
        // 씬에 있는 StageManager를 자동으로 찾아서 연결합니다.
        stageManager = FindAnyObjectByType<StageManager>();
    }

    // 플레이어가 열쇠에 다가와서 조작했을 때 호출할 함수
    public void PickUpKey()
    {
        if (stageManager != null)
        {
            // StageManager의 상태를 HasKey(열쇠 보유)로 변경
            stageManager.ObtainKey();

            Debug.Log("<color=cyan>[아이템]</color> 열쇠를 획득했습니다! 다음 구역 트리거가 활성화됩니다.");

            // 열쇠 획득했으니 맵에서 오브젝트 삭제
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("씬에 StageManager가 없습니다! 맵에 배치했는지 확인해 주세요.");
        }
    }

    // [테스트용 라이트 기능] 플레이어가 가까이 오면 부딪혀서 먹어지게 만들고 싶을 때 사용
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpKey();
        }
    }
}