using UnityEngine;

public class GameObjectActivator : MonoBehaviour
{
    public GameObject objetoParaControlar;

    void Start()
    {
        if (objetoParaControlar != null)
            objetoParaControlar.SetActive(false);
    }

    public void AtivarObjeto()
    {
        if (objetoParaControlar != null)
        {
            objetoParaControlar.SetActive(true);
         
        }
    }

    public void DesativarObjeto()
    {
        if (objetoParaControlar != null)
        {
            objetoParaControlar.SetActive(false);
        
        }
    }
}
