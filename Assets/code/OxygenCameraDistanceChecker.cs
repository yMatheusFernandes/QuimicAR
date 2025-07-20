using UnityEngine;

public class OxygenCameraDistanceChecker : MonoBehaviour
{
    [Header("Configuração")]
    public float maxCameraDistance = 20f;

    private void Update()
    {
        if (Camera.main == null)
        {
 
            return;
        }

        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
    

        if (distance <= maxCameraDistance)
        {
           
        }
        else
        {
            
        }
    }
}
