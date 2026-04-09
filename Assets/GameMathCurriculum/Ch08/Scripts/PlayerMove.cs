using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("=== 이동 설정 ===")]
    [SerializeField] private float moveSpeed      = 5f;
    [SerializeField] private float rotationSpeed  = 10f;  

    private PlayerInput playerInput;
    private Rigidbody   rb;
   

     

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb          = GetComponent<Rigidbody>();
      

        rb.freezeRotation = true; 
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    { 
        Transform cam = Camera.main.transform;

        Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
        Vector3 camRight   = new Vector3(cam.right.x,   0f, cam.right.z  ).normalized;

        Vector3 moveDir = camForward * playerInput.move
                        + camRight   * playerInput.strafe;

        bool isMoving = moveDir.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            moveDir.Normalize();

            
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

             
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime)
            );
        }

     
         
    }
}
