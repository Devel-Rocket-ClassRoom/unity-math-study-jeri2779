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

    // Update is called once per frame
    void Update()
    {
        //switch문으로 상태에 따른 행동을 구분
        switch(currentState)
        {
            case state.Idle:
                HandleIdle();
                break;
            case state.Dragging:
                HandleDragging();
                break;
            case state.Dropping:
                HandleDropping();
                
                break;
        }   
    }

    private void HandleIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
           
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, DragObject))
            {
                if (hit.collider.gameObject != gameObject) return;

                //드롭존 밖에서 시작할 때만 복귀 지점 갱신
                if (!dropZone.IsInZone())
                    startPos = transform.position;

                currentState = state.Dragging;
            }
          
        }
    }

    private void HandleDragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
        {
            tgtPos = hit.point + Vector3.up * yOffset;
            transform.position = tgtPos;
        }
      
        if (Input.GetMouseButtonUp(0))
        {
            smoothDampVelocity = Vector3.zero;
            currentState = state.Dropping;
            if(dropZone.IsInZone())
            {
                tgtPos = dropZone.GetCenterPosition() + Vector3.up * yOffset;
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
        Ray ray = new Ray(smoothed + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            smoothed.y = hit.point.y + yOffset;
        }

        transform.position = smoothed;

        //Y는 터레인 보정으로 미세하게 달라지므로 XZ 거리만으로 도착 판정
        float xzDist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(tgtPos.x, tgtPos.z));

        if (xzDist < 0.01f)
        {
            transform.position = tgtPos;
            currentState = state.Idle;
        }
    }

}
