using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 씬에 빈 오브젝트를 만들어 이 스크립트만 붙이면 됩니다.
// Canvas와 흰색/검은색 Image는 코드에서 자동 생성하므로 별도 UI 세팅이 필요 없습니다.
public class CrushScreenFlash : MonoBehaviour
{
    public static CrushScreenFlash Instance { get; private set; }

    private Canvas flashCanvas;
    private Image whiteImage;
    private Image blackImage;

    void Awake()
    {
        // 싱글톤 등록
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildUI();
    }

    // 풀스크린 Canvas + 흰색/검은색 Image를 코드로 생성
    void BuildUI()
    {
        GameObject canvasObj = new GameObject("CrushFlashCanvas");
        canvasObj.transform.SetParent(transform);

        flashCanvas = canvasObj.AddComponent<Canvas>();
        flashCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        flashCanvas.sortingOrder = 1000; // 다른 UI보다 항상 위에 보이도록

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        whiteImage = CreateFullscreenImage(canvasObj.transform, "WhiteFlash", Color.white);
        blackImage = CreateFullscreenImage(canvasObj.transform, "BlackFlash", Color.black);

        // 시작 시 둘 다 안 보이게
        SetAlpha(whiteImage, 0f);
        SetAlpha(blackImage, 0f);
    }

    Image CreateFullscreenImage(Transform parent, string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);

        Image img = obj.AddComponent<Image>();
        img.color = color;

        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return img;
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    // PlayerCrush 등 외부에서 호출하는 진입점.
    // duration: 전체 깜빡임 지속 시간
    // startInterval: 처음 깜빡임 간격(느림)
    // endInterval: 마지막 깜빡임 간격(빠름)
    // onComplete: 깜빡임이 끝난 뒤 실행할 동작(예: 씬 리로드)
    public void PlayFlash(float duration, float startInterval, float endInterval, System.Action onComplete)
    {
        StartCoroutine(FlashRoutine(duration, startInterval, endInterval, onComplete));
    }

    IEnumerator FlashRoutine(float duration, float startInterval, float endInterval, System.Action onComplete)
    {
        float elapsed = 0f;
        bool showWhite = true; // 흰색부터 시작

        while (elapsed < duration)
        {
            // 진행률(0~1)에 따라 깜빡임 간격을 startInterval -> endInterval로 선형 보간 (점점 빨라짐)
            float progress = elapsed / duration;
            float currentInterval = Mathf.Lerp(startInterval, endInterval, progress);

            if (showWhite)
            {
                SetAlpha(whiteImage, 1f);
                SetAlpha(blackImage, 0f);
            }
            else
            {
                SetAlpha(whiteImage, 0f);
                SetAlpha(blackImage, 1f);
            }

            showWhite = !showWhite;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        // 깜빡임 종료 후 화면 정리
        SetAlpha(whiteImage, 0f);
        SetAlpha(blackImage, 0f);

        onComplete?.Invoke();
    }
}