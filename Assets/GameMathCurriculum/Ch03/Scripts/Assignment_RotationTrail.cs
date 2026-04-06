// =============================================================================
// Assignment_RotationTrail.cs
// -----------------------------------------------------------------------------
// Matrix4x4로 회전 변환 행렬을 생성하고 끝점의 궤적을 추적하는 시스템
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Assignment_RotationTrail : MonoBehaviour
{

    [Header("=== 회전 설정 ===")]
    [Tooltip("회전 반경 (원점에서 끝점까지)")]
    [Range(1f, 5f)]
    [SerializeField] private float armLength = 2f;

    [Tooltip("회전 각도 (Z축 기준)")]
    [Range(0f, 360f)]
    [SerializeField] private float rotationAngle = 0f;

    [Header("=== 궤적 설정 ===")]
    [Tooltip("기록할 궤적의 최대 길이 (프레임 수)")]
    [Range(5, 60)]
    [SerializeField] private int trailLength = 30;

    [Tooltip("궤적의 색상")]
    [SerializeField] private Color trailColor = new Color(0f, 1f, 1f, 1f);

    [Header("=== 자동 회전 ===")]
    [Tooltip("자동 회전 속도 (도/초)")]
    [Range(30f, 360f)]
    [SerializeField] private float rotationSpeed = 120f;

    [Tooltip("자동 회전을 활성화할지 여부")]
    [SerializeField] private bool autoRotate = true;

    [Header("=== UI 연결 ===")]
    [Tooltip("궤적 정보를 표시할 TMP_Text")]
    [SerializeField] private TMP_Text uiText;

    private List<Vector3> trailPositions = new List<Vector3>();
    private Vector3 lastTipPos;

    private void Update()
    {

       
       

       
        if (autoRotate) //autoRotate시 angle을 지속적 증가시키고 angle이 360도가 되면 0으로 초기화 
        {
            rotationAngle += rotationSpeed * Time.deltaTime; 
            rotationAngle %= 360f;  
        }

        
        //trs 기준 
        Vector3 rotateLen = new Vector3(armLength, 0f, 0f);  // 원점과 끝점의 로컬 위치 정의
        
        Quaternion rotQuat = Quaternion.Euler(0f, 0f, rotationAngle); // angle에 따라 회전하는 끝점의 월드 좌표 

        Vector3 scale = new Vector3(1f, 1f, 1f);  

        
        Matrix4x4 rotateMat = Matrix4x4.TRS(Vector3.zero, rotQuat, scale);//z축 회전
        
        Vector3 originWorld = transform.position + rotateMat.MultiplyPoint3x4(rotateLen);//회전 행렬을 원점에 적용하여 끝점의 월드 좌표 계산
       
        lastTipPos = originWorld;  //  그려진 선을 끝점의 월드 좌표로 업데이트하여 회전 궤적 시각화
        
        trailPositions.Add(originWorld);//끝점에 trail궤적을 추가하고 trailLength를 초과시 오래된순으로 제거
        if (trailPositions.Count > trailLength)
        {
            
            trailPositions.RemoveAt(0);

        }
        UpdateUI();
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, lastTipPos);

        if (trailPositions.Count > 1)
        {
            for (int i = 0; i < trailPositions.Count - 1; i++)
            {
                float alpha = (float)i / trailPositions.Count;
                Color fadeColor = new Color(trailColor.r, trailColor.g, trailColor.b, alpha);

                Gizmos.color = fadeColor;
                Gizmos.DrawLine(trailPositions[i], trailPositions[i + 1]);
            }
        }
    }

    private void UpdateUI()
    {
        if (uiText == null) return;

        uiText.text =
            $"[과제] Matrix4x4 회전 궤적\n" +
            $"회전 pos: {armLength:F2}\n" +
            $"회전 Angle: {rotationAngle:F1}°\n" +
            $"궤적 Length: {trailPositions.Count} / {trailLength}\n" +
            $"회전 speed: {rotationSpeed:F0}°/sec";
    }
}
