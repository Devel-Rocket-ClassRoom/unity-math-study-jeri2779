using UnityEngine;

public class BeizierRandomMover : MonoBehaviour
{
    [Header("=== 스폰 설정 ===")]
    [SerializeField] private int spawnCount = 10;
    [Range(0.1f, 2f)]
    [SerializeField] private float sphereSize = 1f;

    [Header("=== 시작점/끝점 ===")]
    [SerializeField] private Transform startPointTransform;
    [SerializeField] private Transform endPointTransform;

    [Header("=== 중간점 랜덤 범위 ===")]
    [SerializeField] private float checkPointRangeMin = -5f;
    [SerializeField] private float checkPointRangeMax = 5f;

    [Header("=== 이동 시간 ===")]
    [Range(0.5f, 10f)]
    [SerializeField] private float durationMin = 1f;

    [Range(0.5f, 10f)]
    [SerializeField] private float durationMax = 3f;

    [Header("=== 트레일 설정 ===")]
    [Range(0.1f, 2f)]
    [SerializeField] private float trailTime = 0.5f;

    [Range(0.05f, 0.5f)]
    [SerializeField] private float trailWidth = 0.1f;

    [Header("=== 가시성 설정 ===")]
    [SerializeField] private bool showSphereMesh = true;
    [Header("=== 체크포인트 비율 ===")]
    [Range(0f, 1f)]
    [SerializeField] private float p1Point = 0.33f;

    [Range(0f, 1f)]
    [SerializeField] private float p2Point = 0.66f;

    [SerializeField] private Material trailMaterial;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnLine();
    }

    private void SpawnLine()
    {
       

        Vector3 actualStartPos = startPointTransform.position;
        Vector3 actualEndPos = endPointTransform.position;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);//GameObject생성용 
            sphere.name = $"trb{i}";
            sphere.transform.localScale = Vector3.one * sphereSize;//크기조절

            Color randomColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);//랜덤 색상 설정

            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.material.color = randomColor;
            meshRenderer.enabled = showSphereMesh;

            TrailRenderer trail = sphere.AddComponent<TrailRenderer>();
            trail.time = trailTime;                                 //트레일 지속 시간 설정
            trail.startWidth = trailWidth * sphereSize;             //트레일 시작 두꼐 설정
            trail.endWidth = trail.startWidth * 0.2f;               //트레일 끝 두꼐 설정
            trail.material = trailMaterial;                         //트레일 머티리얼 설정
            trail.startColor = randomColor;                         //트레일 시작 색상 설정
            trail.endColor = new Color
               (randomColor.r,
                randomColor.g, 
                randomColor.b, 0f);                                 //트레일 끝 색상 설정
            BezierObject agent = sphere.AddComponent<BezierObject>();

            Vector3 p1 = Vector3.Lerp(actualStartPos, actualEndPos, p1Point) + RandomPoint();  //1번째 체크포인트 계산
            Vector3 p2 = Vector3.Lerp(actualStartPos, actualEndPos, p2Point) + RandomPoint();  //2번째 체크포인트 계산
            float duration = Random.Range(durationMin, durationMax);//이동 시간 랜덤 설정

            agent.Initialize(actualStartPos, p1, p2, actualEndPos, duration);//BezierObject 초기화
        }
    }
    private Vector3 RandomPoint() => new Vector3(
        Random.Range(checkPointRangeMin, checkPointRangeMax),//X축 랜덤  
        Random.Range(checkPointRangeMin, checkPointRangeMax),//Y축 랜덤  
        Random.Range(checkPointRangeMin, checkPointRangeMax)//Z축 랜덤  
    ); 
}
