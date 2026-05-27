using UnityEngine;

// 파일 이름이 FlashlightController1 이므로 클래스 이름도 똑같이 맞춰줬습니다!
public class FlashlightController1 : MonoBehaviour
{
    [Header("Flashlight Setup")]
    public Light flashlight;
    public KeyCode toggleKey = KeyCode.F;

    private bool isLightOn = false;

    void Start()
    {
        if (flashlight != null)
            flashlight.enabled = isLightOn;
    }

    // 딱 하나만 존재해야 하는 Update 함수!
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isLightOn = !isLightOn;
            flashlight.enabled = isLightOn;
        }
    }
}