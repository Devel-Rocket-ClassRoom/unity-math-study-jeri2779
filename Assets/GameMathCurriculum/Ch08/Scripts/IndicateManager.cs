using UnityEngine;

public class IndicateManager : MonoBehaviour
{
    public Transform[] target;
    public indicatorchase[] indicators;
 
  
     
    void Start()
    {
        for (int i = 0; i < target.Length; i++)
        {
            indicators[i].target = target[i];
        }
            
    }

   
}
