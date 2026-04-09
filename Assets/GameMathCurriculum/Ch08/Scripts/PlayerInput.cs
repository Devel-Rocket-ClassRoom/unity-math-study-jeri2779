using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string moveAxis   = "Vertical";
    public static readonly string strafeAxis = "Horizontal";

    public float move   { get; private set; }
    public float strafe { get; private set; }

    private void Update()
    {
        move   = Input.GetAxisRaw(moveAxis);
        strafe = Input.GetAxisRaw(strafeAxis);
    }
}
