using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interact Settings")]
    public Camera mainCamera;
    public float interactRange = 3f;

    // Update는 매 프레임마다 실행되는 유니티 기본 함수입니다.
    // 여기서 CheckInteraction()을 부르고 있기 때문에 'never used' 에러가 사라집니다!
    void Update()
    {
        CheckInteraction();
    }

    // 우리가 만든 상호작용 레이저 발사 기능
    void CheckInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // 레이저에 맞은 물체(또는 그 부모)에게서 Door 컴포넌트를 찾음
                    Door door = hit.collider.GetComponentInParent<Door>();

                    if (door != null)
                    {
                        door.ToggleDoor(); // 문 열고 닫기 실행!
                    }
                }
            }
        }
    }
}