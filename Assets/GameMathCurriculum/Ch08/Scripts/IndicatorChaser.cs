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

 
        bool isOnScreen =
            !isBehind &&
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
 
        float padding = 40f;
        float halfW = center.x - padding;
        float halfH = center.y - padding;

      
        float scaleX = halfW / Mathf.Abs(dir.x);
        float scaleY = halfH / Mathf.Abs(dir.y);
        float scale = Mathf.Min(scaleX, scaleY);

        Vector2 clampedPos = dir * scale;
        Vector2 finalScreenPos = center + clampedPos;

    
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            finalScreenPos,
            null,
            out localPos
        );
        indicator.localPosition = localPos;

        // 방향으로 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        indicator.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}