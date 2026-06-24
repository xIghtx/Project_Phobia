using UnityEngine;

public class ShrinkingWalls : MonoBehaviour
{
    public bool isShrinking = false;

    [Header("중심 기준으로 키울 양쪽 벽 (Transform)")]
    // [중요] 유니티 큐브를 그냥 넣지 마시고, '2단계'에서 만들 'Pivot 빈 오브젝트'를 넣어주세요!
    public Transform leftWallPivot;
    public Transform rightWallPivot;

    [Header("이 복도의 원래 너비 (중앙 통로 폭)")]
    // 중앙 복도 / 좌우 복도 = 7,  상하 복도 = 4
    // 절대 속도는 복도마다 다르지만, "원래 너비 대비 줄어드는 비율(%)"은 동일하게 맞춰집니다.
    public float corridorWidth = 7f;

    [Header("초당 줄어드는 비율 (%) - 0.1 = 초당 10%")]
    // 모든 복도에 이 값을 동일하게 넣으면, 너비가 달라도 상대적인 압박 비율은 같아집니다.
    public float shrinkSpeed = 0.1f;

    void Update()
    {
        // 밸브를 잠갔거나 함정이 안 켜졌으면 작동 안 함
        if (!isShrinking) return;

        // [핵심 로직] 위치 보정 코드 전부 삭제!
        // 그냥 localScale을 늘립니다.
        // 유니티는 기본적으로 오브젝트의 중심(Pivot)을 기준으로 사방으로 대칭되게 스케일이 커집니다!

        // [비율 보정] corridorWidth를 곱해서, 너비가 넓은 복도(7)는 절대 속도가 더 빠르고
        // 좁은 복도(4)는 절대 속도가 더 느리지만, "원래 너비 대비 비율"은 동일하게 줄어듭니다.
        // 예: shrinkSpeed = 0.1 → 너비 7 복도는 초당 0.7만큼, 너비 4 복도는 초당 0.4만큼 늘어나며
        //     둘 다 "원래 너비의 10%"라는 같은 비율로 좁아짐.
        float increaseAmount = corridorWidth * shrinkSpeed * Time.deltaTime;
        Vector3 scaleIncrease = new Vector3(increaseAmount, 0, increaseAmount);

        if (leftWallPivot != null)
        {
            leftWallPivot.localScale += scaleIncrease;
        }

        if (rightWallPivot != null)
        {
            rightWallPivot.localScale += scaleIncrease;
        }
    }
}