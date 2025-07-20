using UnityEngine;

public class OxygenAnimationTrigger : MonoBehaviour
{
    public DistanceCheckerByTag distanceChecker;

    [Header("Materiais do Oxigênio")]
    public Material normalMaterial;
    public Material fusingMaterial;
    public Material fusedCameraCloseMaterial;

    [Header("Outro Objeto")]
    public GameObject otherObjectToChange;
    public Material otherNormalMaterial;
    public Material otherFusingMaterial;
    public Material otherFusedCameraCloseMaterial;

    [Header("Objeto a Ativar Quando Câmera Estiver Perto")]
    public GameObject objectToActivateWhenCameraIsClose;

    private Renderer rend;
    private Renderer otherRenderer;
    private Animator animator;
    private bool isFusing = false;
    private bool isCameraCloseWhileFused = false;

    [Header("Distância da Câmera")]
    public float cameraTriggerDistance = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();

        if (rend != null && normalMaterial != null)
            rend.material = normalMaterial;

        if (otherObjectToChange != null)
        {
            otherRenderer = otherObjectToChange.GetComponent<Renderer>();
            if (otherRenderer != null && otherNormalMaterial != null)
                otherRenderer.material = otherNormalMaterial;
        }

        // Garante que o objeto comece desativado
        if (objectToActivateWhenCameraIsClose != null)
            objectToActivateWhenCameraIsClose.SetActive(false);
    }

    public void Perdeu()
    {
        ApplyNormalMaterials();
        isFusing = false;
        isCameraCloseWhileFused = false;

        if (objectToActivateWhenCameraIsClose != null)
            objectToActivateWhenCameraIsClose.SetActive(false);

    
    }

    void Update()
    {
        if (distanceChecker == null || rend == null || Camera.main == null)
            return;

        float cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (distanceChecker.AreTwoHydrogensCloseToOxygen())
        {
            if (!isFusing)
            {
                isFusing = true;
                animator.SetBool("isFusing", true);
           

                ApplyFusingMaterials();
                isCameraCloseWhileFused = false;
            }
        }
        else
        {
            if (isFusing)
            {
                isFusing = false;
                animator.SetBool("isFusing", false);
         

                ApplyNormalMaterials();
                isCameraCloseWhileFused = false;

                if (objectToActivateWhenCameraIsClose != null)
                    objectToActivateWhenCameraIsClose.SetActive(false);
            }
        }

       
        if (isFusing)
        {
            if (cameraDistance <= cameraTriggerDistance)
            {
                if (!isCameraCloseWhileFused)
                {
                    ApplyFusedCameraCloseMaterials();
                    isCameraCloseWhileFused = true;

                    if (objectToActivateWhenCameraIsClose != null)
                        objectToActivateWhenCameraIsClose.SetActive(true);

                
                }
            }
            else
            {
                if (isCameraCloseWhileFused)
                {
                    ApplyFusingMaterials();
                    isCameraCloseWhileFused = false;

                    if (objectToActivateWhenCameraIsClose != null)
                        objectToActivateWhenCameraIsClose.SetActive(false);

                    
                }
            }
        }
    }

    void ApplyNormalMaterials()
    {
        if (rend != null && normalMaterial != null)
            rend.material = normalMaterial;

        if (otherRenderer != null && otherNormalMaterial != null)
            otherRenderer.material = otherNormalMaterial;
    }

    void ApplyFusingMaterials()
    {
        if (rend != null && fusingMaterial != null)
            rend.material = fusingMaterial;

        if (otherRenderer != null && otherFusingMaterial != null)
            otherRenderer.material = otherFusingMaterial;
    }

    void ApplyFusedCameraCloseMaterials()
    {
        if (rend != null && fusedCameraCloseMaterial != null)
            rend.material = fusedCameraCloseMaterial;

        if (otherRenderer != null && otherFusedCameraCloseMaterial != null)
            otherRenderer.material = otherFusedCameraCloseMaterial;
    }
}
