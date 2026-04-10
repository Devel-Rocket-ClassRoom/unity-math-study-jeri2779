using UnityEngine;

public class indicatorchase : MonoBehaviour
{
    public Transform target;
    public RectTransform indicator;

    Camera cam;
    RectTransform canvasRect;
    CanvasGroup cg;

    void Start()
    {
        cam = Camera.main;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        cg = indicator.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (target == null || indicator == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        // 카메라 뒤에 있으면 반전
        bool isBehind = screenPos.z < 0f;
        if (isBehind)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

 
        bool isOnScreen = !isBehind &&
            screenPos.x > 0f && screenPos.x < Screen.width &&
            screenPos.y > 0f && screenPos.y < Screen.height;

        if (isOnScreen)
        {
            cg.alpha = 0f; // 화면 안에 있으면 투명하게
            return;
        }
 
        cg.alpha = 1f; // 화면 밖에 있으면 표시
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = new Vector2(screenPos.x - center.x, screenPos.y - center.y);

        float padding = 40f;        //중앙기준 설정값만큼 떨어지게 설정.
        float halfWidth = center.x - padding;
        float halfHeight = center.y - padding;


        float scaleX = halfWidth / Mathf.Abs(dir.x);
        float scaleY = halfHeight / Mathf.Abs(dir.y);
        float scale = Mathf.Min(scaleX, scaleY);        //화면 밖으로 나가지 않도록 조정

        Vector2 clampedPos = dir * scale;
        Vector2 finalScreenPos = center + clampedPos;   //중앙기준 조정된 위치로 계산

        // 스크린 좌표 -> 캔버스 로컬 좌표
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, finalScreenPos, null, out localPos);
        indicator.localPosition = localPos;

        // 방향으로 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        indicator.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}