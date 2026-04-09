 
using UnityEngine;
using TMPro;

public class FollowCam : MonoBehaviour
{
    [Header("=== 추적 대상 ===")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3   offset = new Vector3(0f, 5f, -8f);

    [Header("=== SmoothDamp 설정 ===")]
    [Range(0.01f, 1f)] [SerializeField] private float positionSmoothTime  = 0.15f;
    [Range(0.01f, 1f)] [SerializeField] private float zoomSmoothTime      = 0.2f;
    [Range(1f, 20f)]   [SerializeField] private float rotationSmoothSpeed = 15f;

    [Header("=== 줌 설정 ===")]
    [Range(2f, 10f)]   [SerializeField] private float minZoomDistance = 3f;
    [Range(10f, 30f)]  [SerializeField] private float maxZoomDistance = 15f;
    [Range(1f, 10f)]   [SerializeField] private float zoomSpeed       = 3f;

    [Header("=== 마우스 감도 ===")]
    [SerializeField] private float sensitivity = 150f;
    [SerializeField] private float minXRot     = -40f;
    [SerializeField] private float maxXRot     = 60f;

    [Header("=== UI 연결 ===")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 (읽기 전용) ===")]
    [SerializeField] private float currentZoomDistance = 10f;
    [SerializeField] private Vector3 currentSmoothVelocity;
    [SerializeField] private float   zoomVelocity;

 
    private float xRotation;
    private float yRotation;
    private float targetZoomDistance;
    private Vector3 positionVelocity;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("[FollowCam] Target이 지정되지 않았습니다!");
            enabled = false;
            return;
        }

        targetZoomDistance = currentZoomDistance; 
        Cursor.lockState   = CursorLockMode.Locked;
        Cursor.visible     = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            targetZoomDistance -= scroll * zoomSpeed;
                    targetZoomDistance  = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
        }
        currentZoomDistance = Mathf.SmoothDamp(
            currentZoomDistance, targetZoomDistance, ref zoomVelocity, zoomSmoothTime);

     
        yRotation += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        xRotation -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRotation  = Mathf.Clamp(xRotation, minXRot, maxXRot);
 
        Quaternion camRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Vector3    targetPos   = target.position
                               + camRotation * offset.normalized * currentZoomDistance;

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPos, ref positionVelocity, positionSmoothTime);

        
        Quaternion targetRot = Quaternion.LookRotation(-(camRotation * offset.normalized));
        transform.rotation   = Quaternion.Slerp(
            transform.rotation, targetRot, rotationSmoothSpeed * Time.deltaTime);

        currentSmoothVelocity = positionVelocity;
    }
}