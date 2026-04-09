using UnityEngine;

public class DropZone : MonoBehaviour
{
    private bool isOnjectInZone = false;
    private GameObject target;
 
 

    public bool IsInZone()
    {
        if(isOnjectInZone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector3 GetCenterPosition()
    {
        if (isOnjectInZone)
        {
            Vector3 centerPosition = transform.position;
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                centerPosition.y = col.bounds.max.y;//콜라이더 y값의 제일 위로 설정 

            }
            return centerPosition;

        }
        return transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DragObject"))
        {
            isOnjectInZone = true;
            Debug.Log("Object entered the drop zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DragObject"))
        {
            isOnjectInZone = false;
            Debug.Log("Object exited the drop zone.");
        }
    }
}