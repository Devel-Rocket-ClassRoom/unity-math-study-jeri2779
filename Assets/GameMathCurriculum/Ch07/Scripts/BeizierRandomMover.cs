using System.Collections;
using UnityEngine;

public class BeizierRandomMover : MonoBehaviour
{
    [Header("=== 스폰 설정 ===")]
    [Tooltip("생성될 객체 갯수")]
    [SerializeField] private int spawnCount = 10;

    [Header("=== 경로 설정 ===")]
    [Tooltip("시작점 (P0)")]
    [SerializeField] private Vector3 startPos = new Vector3(-10f, 0f, 0f);

    [Tooltip("끝점 (P3)")]
    [SerializeField] private Vector3 endPos = new Vector3(10f, 3f, 0f);

    [Tooltip("P1, P2 랜덤 오프셋 범위")]
    [SerializeField] private float controlPointRange = 5f;

    [Header("=== 이동 시간 ===")]
    [Range(0.5f, 10f)]
    [SerializeField] private float durationMin = 1f;

    [Range(0.5f, 10f)]
    [SerializeField] private float durationMax = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnAll();
    }

    private void SpawnAll()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Sphere 생성
            GameObject sphere   = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name         = $"BezierSphere_{i}";
            sphere.transform.position = startPos;

            //   P1, P2 랜덤 제어점 생성
            Vector3 p1       = startPos + RandomOffset();
            Vector3 p2       = endPos   + RandomOffset();

            //  랜덤 이동 시간
            float duration   = Random.Range(durationMin, durationMax);

            //   코루틴으로 이동 위임 — BezierAgent 없이 처리
            StartCoroutine(MoveSphere(sphere, startPos, p1, p2, endPos, duration));
        }
    }

    /// <summary>
    /// Sphere 하나를 베지어 곡선을 따라 이동시키고 도착 시 파괴.
    /// BezierAgent.Update() 역할을 코루틴으로 대체.
    /// </summary>
    private IEnumerator MoveSphere(
        GameObject sphere,
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // BezierCurveDemo.elapsedTime 패턴 → Clamp01 방식
            elapsed  += Time.deltaTime;
            float t   = Mathf.Clamp01(elapsed / duration);

            sphere.transform.position = CubicBezier(p0, p1, p2, p3, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 끝점 도착 → 자동 파괴
        Destroy(sphere);
    }

    // BezierCurveDemo.CubicBezier() 재활용
    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0
             + 3f * u * u * t  * p1
             + 3f * u * t  * t * p2
             +      t * t  * t * p3;
    }

    private Vector3 RandomOffset() => new Vector3(
        Random.Range(-controlPointRange, controlPointRange),
        Random.Range(-controlPointRange, controlPointRange),
        Random.Range(-controlPointRange, controlPointRange)
    );
}
