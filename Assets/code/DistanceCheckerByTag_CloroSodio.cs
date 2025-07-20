using UnityEngine;

public class DistanceCheckerByTag_CloroSodio : MonoBehaviour
{
    public string cloroTag = "Cloro";
    public string sodioTag = "Sodio";
    public float maxDistance = 1.4f;

    /// <summary>
    /// Retorna true se um cloro ativo e um sódio ativo estiverem a menos de maxDistance um do outro.
    /// </summary>
    public bool IsCloroCloseToSodio()
    {
        GameObject[] cloros = GameObject.FindGameObjectsWithTag(cloroTag);
        GameObject[] sodios = GameObject.FindGameObjectsWithTag(sodioTag);

        if (cloros.Length == 0 || sodios.Length == 0)
        {
            Debug.LogWarning("❗ Precisa de pelo menos 1 cloro e 1 sódio na cena!");
            return false;
        }

        GameObject cloro = null;
        GameObject sodio = null;

        // Encontra o primeiro cloro ativo
        foreach (GameObject c in cloros)
        {
            if (c.activeInHierarchy)
            {
                cloro = c;
                break;
            }
        }

        // Encontra o primeiro sódio ativo
        foreach (GameObject s in sodios)
        {
            if (s.activeInHierarchy)
            {
                sodio = s;
                break;
            }
        }

        if (cloro == null || sodio == null)
        {
            Debug.LogWarning("❗ Cloro ou sódio não estão sendo rastreados (ativos).");
            return false;
        }

        float distance = Vector3.Distance(cloro.transform.position, sodio.transform.position);
        Debug.Log($"🔍 Distância entre '{cloro.name}' e '{sodio.name}': {distance:F2}");

        if (distance <= maxDistance)
        {
            Debug.Log("✅ Cloro e sódio estão próximos o suficiente!");
            return true;
        }

        Debug.Log("❌ Cloro e sódio não estão próximos o suficiente.");
        return false;
    }

}
