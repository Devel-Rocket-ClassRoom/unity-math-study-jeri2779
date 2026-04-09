using UnityEngine;

public class DragNDrop : MonoBehaviour
{ 
    public DropZone dropZone;
    public LayerMask terrainLayer;
    public LayerMask DragObject;
    enum state {  Idle, Dragging, Dropping }

    private state currentState = state.Idle;

    private Vector3 startPos;
    private Vector3 tgtPos;

    private Vector3 smoothDampVelocity;
    private float yOffset;

    public float returnDuration = 2f; 

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            yOffset = col.bounds.extents.y;

        }

        //transform.position 이동 시 OnTriggerEnter 감지에 Rigidbody 필요
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    
    void Update()
    {
        //switch문으로 상태에 따른 행동을 구분
        switch(currentState)
        {
            case state.Idle:
                HandleIdle(); //마우스 클릭시 Dragging 상태로 전환
                break;
            case state.Dragging:
                HandleDragging(); // 마우스 위치따라 이동 , 떼면 Dropping 상태전환
                break;
            case state.Dropping:
                HandleDropping();// SmoothDamp이용하여 이동, 도착하면 Idle 상태전환

                break;
        }   
    }
    //ray는 총 세 번 발사됨. 첫 번째는 드래그 시작 시, 두 번째는 드래그 중, 세 번째는 드롭 후 이동 시
    private void HandleIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//DragObject용 ray.
            RaycastHit hit;
           
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, DragObject))// 레이캐스트 확인
            {
                if (hit.collider.gameObject != gameObject) return;

                //dropzone 밖에서 시작할 때 복귀 지점 갱신
                if (!dropZone.IsInZone())
                    startPos = transform.position;

                currentState = state.Dragging;
            }
          
        }
    }

    private void HandleDragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Terrain용 ray
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
        {
            tgtPos = hit.point + Vector3.up * yOffset;
            transform.position = tgtPos;
        }
      
        if (Input.GetMouseButtonUp(0))
        {
            smoothDampVelocity = Vector3.zero;
            currentState = state.Dropping;   //드롭 상태로 전환
            if (dropZone.IsInZone())
            {
                tgtPos = dropZone.GetCenterPosition() + Vector3.up * yOffset;       //존 중심으로 이동
            }
            else
            {
                tgtPos = startPos;
            }
            return;
        }
    }
    private void HandleDropping()
    {
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, tgtPos, ref smoothDampVelocity, returnDuration);

        //터레인 표면을 따라 이동
        Ray ray = new Ray(smoothed + Vector3.up * 100f, Vector3.down);//
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            smoothed.y = hit.point.y + yOffset;                     //  y값 조정
        }

        transform.position = smoothed;

        //Y는 terrain 표면에 맞춰 조정되므로 xz만 계산
        float checkDist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(tgtPos.x, tgtPos.z));

        if (checkDist < 0.01f)                     //smoothed가 tgtPos에 거의 도달했는지 확인(임계값 체크용)
        {
            transform.position = tgtPos;        //정확히 tgtPos로 이동
            currentState = state.Idle;          //Idle 상태전환
        }
    }

}
