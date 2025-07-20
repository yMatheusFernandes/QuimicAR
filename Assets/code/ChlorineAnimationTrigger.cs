using UnityEngine;

public class ChlorineFusionController : MonoBehaviour
{
    [Header("Verificador de Fusão (Cloro + Sódio)")]
    public DistanceCheckerByTag_CloroSodio distanceChecker;

    [Header("Mesh e Materiais do Cloro")]
    public Mesh normalMesh;
    public Material normalMaterial;
    public Mesh fusedMesh;
    public Material fusedMaterial;
    public Mesh fusedCameraMesh;
    public Material fusedCameraMaterial;

    [Header("Outro Objeto")]
    public GameObject otherObjectToChange;
    public Material otherNormalMaterial;
    public Material otherFusedMaterial;
    public Material otherFusedCameraMaterial;

    [Header("Objeto a Ativar Quando Câmera Estiver Perto")]
    public GameObject objectToActivateWhenCameraIsClose;

    [Header("Distância da Câmera para Ação Extra")]
    public float cameraTriggerDistance = 5f;

    private MeshFilter meshFilter;
    private Renderer rend;
    private Renderer otherRenderer;
    private Animator animator;

    private bool isFusing = false;
    private bool isCameraClose = false;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        rend = GetComponent<Renderer>();
        animator = GetComponent<Animator>();

        if (otherObjectToChange != null)
            otherRenderer = otherObjectToChange.GetComponent<Renderer>();

        if (objectToActivateWhenCameraIsClose != null)
            objectToActivateWhenCameraIsClose.SetActive(false);

        ApplyNormalState();
    }

    void Update()
    {
        if (Camera.main == null || distanceChecker == null)
            return;

        float cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (distanceChecker.IsCloroCloseToSodio())
        {
            if (!isFusing)
            {
                isFusing = true;
                animator.SetBool("isFusing", true);
                ApplyFusedState();
            }

            if (cameraDistance <= cameraTriggerDistance)
            {
                if (!isCameraClose)
                {
                    isCameraClose = true;
                    ApplyFusedCameraCloseState();

                    if (objectToActivateWhenCameraIsClose != null)
                        objectToActivateWhenCameraIsClose.SetActive(true);
                }
            }
            else
            {
                if (isCameraClose)
                {
                    isCameraClose = false;
                    ApplyFusedState();

                    if (objectToActivateWhenCameraIsClose != null)
                        objectToActivateWhenCameraIsClose.SetActive(false);
                }
            }
        }
        else
        {
            if (isFusing)
            {
                isFusing = false;
                isCameraClose = false;

                animator.SetBool("isFusing", false);
                ApplyNormalState();

                if (objectToActivateWhenCameraIsClose != null)
                    objectToActivateWhenCameraIsClose.SetActive(false);
            }
        }
    }

    void ApplyNormalState()
    {
        if (meshFilter != null && normalMesh != null)
            meshFilter.mesh = normalMesh;

        if (rend != null && normalMaterial != null)
            rend.material = normalMaterial;

        if (otherRenderer != null && otherNormalMaterial != null)
            otherRenderer.material = otherNormalMaterial;
    }

    void ApplyFusedState()
    {
        if (meshFilter != null && fusedMesh != null)
            meshFilter.mesh = fusedMesh;

        if (rend != null && fusedMaterial != null)
            rend.material = fusedMaterial;

        if (otherRenderer != null && otherFusedMaterial != null)
            otherRenderer.material = otherFusedMaterial;
    }

    void ApplyFusedCameraCloseState()
    {
        if (meshFilter != null && fusedCameraMesh != null)
            meshFilter.mesh = fusedCameraMesh;

        if (rend != null && fusedCameraMaterial != null)
            rend.material = fusedCameraMaterial;

        if (otherRenderer != null && otherFusedCameraMaterial != null)
            otherRenderer.material = otherFusedCameraMaterial;
    }
}
