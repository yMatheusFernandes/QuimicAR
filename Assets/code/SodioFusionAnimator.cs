using UnityEngine;
using Vuforia;

public class SodioAnimatorControl : MonoBehaviour
{
    public Animator animator;
    public GameObject objetoParaDesativar;
    public DistanceCheckerByTag_CloroSodio distanceChecker;
    public ObserverBehaviour cloroTarget;

    private bool isFusing = false;

    void Start()
    {
        if (cloroTarget != null)
        {
            cloroTarget.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void Update()
    {
        if (cloroTarget == null || distanceChecker == null)
            return;

        bool isCloroDetected = cloroTarget.TargetStatus.Status == Status.TRACKED;
        bool isCloroClose = distanceChecker.IsCloroCloseToSodio();

        if (isCloroDetected && isCloroClose)
        {
            if (!isFusing)
            {
                isFusing = true;
                animator.SetBool("isFusing", true);
                Debug.Log("✅ Fundindo: Cloro detectado e sódio próximo!");
            }
        }
        else
        {
            if (isFusing)
            {
                isFusing = false;
                animator.SetBool("isFusing", false);
                Debug.Log("❌ Desfundindo: condição de fusão não atendida.");
            }
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.NO_POSE || status.Status == Status.LIMITED)
        {
            Debug.Log("🛑 Cloro perdido! Resetando fusão...");
            ResetFusion();
        }
    }

    private void ResetFusion()
    {
        isFusing = false;
        animator.SetBool("isFusing", false);

        if (objetoParaDesativar != null)
        {
            objetoParaDesativar.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (cloroTarget != null)
        {
            cloroTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}
