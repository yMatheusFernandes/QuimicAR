using UnityEngine;
using Vuforia;

public class TargetVisibilityHandler : MonoBehaviour
{
    public GameObject objetoParaControlar;

    private void Start()
    {
        // Começa desativado por padrão
        if (objetoParaControlar != null)
        {
            objetoParaControlar.SetActive(false);
        }

        var observerHandler = GetComponent<DefaultObserverEventHandler>();
        if (observerHandler != null)
        {
            observerHandler.OnTargetFound.AddListener(OnTargetFound);
            observerHandler.OnTargetLost.AddListener(OnTargetLost);
        }
        else
        {
           
        }
    }

    private void OnTargetFound()
    {
        AtivarObjeto();
    }

    private void OnTargetLost()
    {
        DesativarObjeto();
    }

    /// <summary>
    /// Ativa o objeto referenciado.
    /// </summary>
    public void AtivarObjeto()
    {
        if (objetoParaControlar != null)
        {
            objetoParaControlar.SetActive(true);
            
        }
    }

    /// <summary>
    /// Desativa o objeto referenciado.
    /// </summary>
    public void DesativarObjeto()
    {
        if (objetoParaControlar != null)
        {
            objetoParaControlar.SetActive(false);
          
        }
    }
}
