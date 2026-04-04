// =============================================================================
// Assignment_EnemyDetector.cs
// -----------------------------------------------------------------------------
// 시야각과 거리 기반으로 적을 탐지하는 시스템
// =============================================================================

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Assignment_EnemyDetector : MonoBehaviour
{

    [Header("=== 감지 설정 ===")]
    [Tooltip("시야각 (전체 각도)")]
    [Range(10f, 360f)]
    [SerializeField] private float detectionFOV = 120f;

    [Tooltip("감지 거리")]
    [Range(1f, 30f)]
    [SerializeField] private float detectionRange = 10f;

    [Header("=== UI 연결 ===")]
    [Tooltip("정보 표시용 TMP_Text (Canvas 하위에 배치)")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [Tooltip("현재 탐지된 적의 수")]
    [SerializeField] private int detectedCount = 0;

    private List<Transform> detectedEnemies = new List<Transform>();

    private GameObject[] allEnemies;

    private void Start()
    {
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"[EnemyDetector] 씬에서 {allEnemies.Length}개의 적을 발견했습니다.");
    }

    private void Update()
    {
        detectedEnemies.Clear();

        foreach (GameObject enemyObj in allEnemies)
        {
            if (enemyObj == null) continue;

            Transform enemy = enemyObj.transform;

            if (IsDetected(enemy))
            {
                detectedEnemies.Add(enemy);
            }

            Renderer renderer = enemyObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = detectedEnemies.Contains(enemy) ? Color.green : Color.red;
            }
        }

        detectedCount = detectedEnemies.Count;
        UpdateUI();
    }

    private bool IsDetected(Transform enemy)
    {


        // TODO

        //방향 벡터 구하기
        Vector3 toEnemy = (enemy.position - transform.position).normalized;

        if(toEnemy.sqrMagnitude > detectionRange * detectionRange) return false;//거리가 범위 박이라면 감지 실패
        //시야각 계산
        Vector3 dir = toEnemy.normalized;
       //dot product로 시야각 계산
        float dot = Vector3.Dot(transform.forward, dir);//dot product로 시야각 계산
        
        float halfFoV = detectionFOV * 0.5f;//시야각을 절반으로 나누기 
        float threshold = Mathf.Cos(halfFoV * Mathf.Deg2Rad);//cos값으로 시야각 계산

        if(dot < threshold) return false;

        return true;









 
    }

       

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Vector3 origin = transform.position;

        VectorGizmoHelper.DrawFOV(origin, transform.forward,
            detectionFOV * 0.5f, detectionRange, new Color(1f, 1f, 0f, 0.3f));

        VectorGizmoHelper.DrawCircleXZ(origin, detectionRange, new Color(1f, 1f, 1f, 0.2f));

        foreach (Transform enemy in detectedEnemies)
        {
            if (enemy != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, enemy.position);
            }
        }
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        uiInfoText.text =
            $"[과제] 적 감지 시스템\n" +
            $"탐지된 적: {detectedCount}마리\n" +
            $"FOV: {detectionFOV}° / 거리: {detectionRange}";
    }
}
