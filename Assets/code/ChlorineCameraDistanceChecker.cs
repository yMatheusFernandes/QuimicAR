using UnityEngine;

public class ChlorineCameraDistanceChecker : MonoBehaviour
{
    [Header("Configuração")]
    public float maxCameraDistance = 20f;

    private void Update()
    {
        if (Camera.main == null)
            return;

        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (distance <= maxCameraDistance)
        {
            // A câmera está perto do Cloro
            // ➤ Adicione aqui a lógica para "próximo"
            Debug.Log(distance);
        }
        else
        {
            // A câmera está longe do Cloro
            // ➤ Adicione aqui a lógica para "longe"
            Debug.Log("📸 Câmera está longe do Cloro.");
        }
    }
}
