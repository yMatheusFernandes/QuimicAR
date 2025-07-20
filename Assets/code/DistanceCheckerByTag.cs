using UnityEngine;

public class DistanceCheckerByTag : MonoBehaviour
{
    public string hydrogenTag = "Hydrogenio";
    public string oxygenTag = "Oxigenio";
    public float maxDistance = 1.4f;

    /// <summary>
    /// Retorna true se dois hidrogênios ativos estiverem a menos de maxDistance do oxigênio ativo.
    /// </summary>
    public bool AreTwoHydrogensCloseToOxygen()
    {
        GameObject[] hydrogens = GameObject.FindGameObjectsWithTag(hydrogenTag);
        GameObject[] oxygens = GameObject.FindGameObjectsWithTag(oxygenTag);

        if (hydrogens.Length < 2 || oxygens.Length == 0)
        {
            Debug.LogWarning("❗ Precisa de pelo menos 2 hidrogênios e 1 oxigênio na cena!");
            return false;
        }

        GameObject oxygen = null;

        // Encontra o primeiro oxigênio ativo
        foreach (GameObject o in oxygens)
        {
            if (o.activeInHierarchy)
            {
                oxygen = o;
                break;
            }
        }

        if (oxygen == null)
        {
            Debug.LogWarning("❗ Nenhum oxigênio está sendo rastreado (ativo).");
            return false;
        }

        int closeHydrogenCount = 0;

        foreach (GameObject hydrogen in hydrogens)
        {
            if (!hydrogen.activeInHierarchy)
            {
                Debug.Log($"⚠️ '{hydrogen.name}' não está sendo rastreado (Vuforia ocultou). Ignorando.");
                continue;
            }

            float distance = Vector3.Distance(hydrogen.transform.position, oxygen.transform.position);
            Debug.Log($"🔍 Distância entre '{hydrogen.name}' e '{oxygen.name}': {distance:F2}");

            if (distance <= maxDistance)
            {
                closeHydrogenCount++;
                if (closeHydrogenCount >= 2)
                {
                    Debug.Log("✅ Dois hidrogênios rastreados estão próximos o suficiente do oxigênio!");
                    return true;
                }
            }
        }

        Debug.Log("❌ Não há dois hidrogênios rastreados próximos do oxigênio.");
        return false;
    }
}
