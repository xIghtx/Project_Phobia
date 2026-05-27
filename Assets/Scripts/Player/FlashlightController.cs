using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Setup")]
    public Light flashlight; // 1단계에서 만든 Spotlight를 여기에 드래그
    public KeyCode toggleKey = KeyCode.F; // F키로 켜고 끄기

    private bool isLightOn = false;

    void Start()
    {
        if (flashlight != null)
            flashlight.enabled = isLightOn;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isLightOn = !isLightOn;
            flashlight.enabled = isLightOn;
        }
    }
}