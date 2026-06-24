using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 게임의 현재 진행 상태를 나누어 둡니다.
    public enum GameProgress
    {
        BeforeKey,  // 열쇠 먹기 전
        HasKey,     // 열쇠 먹음 (세 갈래 길로 갈 준비 완료)
        TimerStart  // 5분 타임어택 시작됨
    }

    [Header("현재 게임 상태")]
    public GameProgress currentProgress = GameProgress.BeforeKey;

    // KeyItem.cs에서 열쇠를 먹었을 때 이 함수를 호출합니다.
    public void ObtainKey()
    {
        currentProgress = GameProgress.HasKey;
        Debug.Log("<color=green>[StageManager]</color> 게임 상태 업데이트: 열쇠 획득 완료! (HasKey)");
    }
}