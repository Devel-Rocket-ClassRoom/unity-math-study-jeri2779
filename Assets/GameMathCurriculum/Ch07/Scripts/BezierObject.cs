using UnityEngine;
 
public class BezierObject : MonoBehaviour
{
  
    private Vector3 pStart, c1, c2, pEnd;
    private float duration;
    
    // BezierCurveDemo 패턴 재활용
    private float elapsedTime;
    private float currentT;
    
 
    public void Initialize(Vector3 start, Vector3 check1, Vector3 check2, Vector3 end, float moveDuration)
    {
        pStart = start;
        c1 = check1;
        c2 = check2;
        pEnd = end;
        duration = moveDuration;
        
        elapsedTime = 0f;
        transform.position = pStart; // 시작 위치로 이동
    }
    
    private void Update()
    {
         
        elapsedTime += Time.deltaTime;
        currentT = Mathf.Clamp01(elapsedTime / duration);
        
      
        transform.position = CubicBezier(pStart, c1, c2 , pEnd, currentT);


        if (currentT >= 1f)
        {
            Destroy(gameObject);
        }
    }
    
    // BezierCurveDemo.CubicBezier() 재활용
    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        var u1 = u * u * u * p0;
        var u2 = u * u * t * p1;
        var u3 = u * t * t * p2;
        var u4 = t * t * t * p3;
        return u1 + 3f * u2 + 3f * u3 + u4;
    }
}
