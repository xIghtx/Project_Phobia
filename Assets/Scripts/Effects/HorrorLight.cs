using System.Collections;
using UnityEngine;

public class HorrorLight : MonoBehaviour
{
    [Header("1. 일반 전구 그룹 (완전히 꺼질 조명들)")]
    public Light[] normalBulbs;

    [Header("2. 비상등 그룹 (붉게 켜질 조명들)")]
    public Light[] emergencyLights;
    public Color emergencyColor = Color.red;

    [Header("강도 세팅")]
    public float normalIntensity = 5.0f;    // 평소 전구 밝기
    public float emergencyIntensity = 4.0f; // 비상등이 켜졌을 때 밝기

    [Header("연출 시간")]
    public float flickerDuration = 2.5f;

    void Start()
    {
        // [초기 세팅] 
        // 일반 전구는 켜두고, 비상등은 완전히 꺼둡니다.
        if (normalBulbs != null)
        {
            foreach (Light bulb in normalBulbs)
            {
                if (bulb != null)
                {
                    bulb.enabled = true;
                    bulb.color = Color.white;
                    bulb.intensity = normalIntensity;
                }
            }
        }

        if (emergencyLights != null)
        {
            foreach (Light emLight in emergencyLights)
            {
                if (emLight != null)
                {
                    emLight.enabled = false; // 평소엔 암전
                    emLight.color = emergencyColor;
                }
            }
        }
    }

    // 트리거에서 호출할 쇼트 연출 코루틴
    public IEnumerator FlickerAndChange()
    {
        float endTime = Time.time + flickerDuration;

        Debug.Log("<color=yellow>[조명]</color> 합선(쇼트) 발생! 전구 파르르 떨림 시작");

        // 1. 쇼트 연출: 일반 전구는 미친 듯이 깜빡이고, 비상등은 들어올까 말까 움찔거림
        while (Time.time < endTime)
        {
            bool isLightOn = Random.value > 0.5f;

            // 일반 전구 깜빡임
            if (normalBulbs != null)
            {
                foreach (Light bulb in normalBulbs)
                {
                    if (bulb != null) bulb.intensity = isLightOn ? Random.Range(normalIntensity * 0.3f, normalIntensity) : 0f;
                }
            }

            // 비상등 움찔거림 (전구랑 반대로 혹은 무작위로 파지직)
            if (emergencyLights != null)
            {
                foreach (Light emLight in emergencyLights)
                {
                    if (emLight != null)
                    {
                        emLight.enabled = Random.value > 0.7f; // 가끔씩만 불꽃 튀듯 켜짐
                        emLight.intensity = Random.Range(1f, emergencyIntensity);
                    }
                }
            }

            yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
        }

        // 2. 연출 끝: 일반 전구는 쇼트로 '완전히 파괴(암전)', 비상등만 '붉게 점등'
        Debug.Log("<color=red>[조명]</color> 일반 전구 전멸. 비상전력 가동!");

        if (normalBulbs != null)
        {
            foreach (Light bulb in normalBulbs)
            {
                if (bulb != null) bulb.enabled = false; // 완전히 꺼짐
            }
        }

        if (emergencyLights != null)
        {
            foreach (Light emLight in emergencyLights)
            {
                if (emLight != null)
                {
                    emLight.enabled = true; // 비상등 온!
                    emLight.intensity = emergencyIntensity;
                }
            }
        }
    }
}