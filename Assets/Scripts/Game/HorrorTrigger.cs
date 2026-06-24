using UnityEngine;

public class HorrorTrigger : MonoBehaviour
{
    private StageManager stageManager;

    [Header("연결할 공포 연출들")]
    public HorrorLight horrorLight;  // 다시 깔끔하게 1개(LightManager)만 받도록 복구!
    public ShrinkingWalls shrinkingWalls;

    void Start()
    {
        stageManager = FindAnyObjectByType<StageManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stageManager != null && stageManager.currentProgress == StageManager.GameProgress.HasKey)
            {
                stageManager.currentProgress = StageManager.GameProgress.TimerStart;
                Debug.Log("<color=red>[경고]</color> 세 갈래 길 진입! 지옥이 시작됩니다.");

                // 매니저를 통해 전체 조명 연출 시작
                if (horrorLight != null)
                {
                    horrorLight.StartCoroutine(horrorLight.FlickerAndChange());
                }

                if (shrinkingWalls != null)
                {
                    shrinkingWalls.isShrinking = true;
                }

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}