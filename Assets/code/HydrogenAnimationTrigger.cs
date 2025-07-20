using UnityEngine;
using Vuforia;

public class HydrogenAnimatorControl : MonoBehaviour
{
    public Animator animator;
    public GameObject objetoParaDesativar;
    public DistanceCheckerByTag distanceChecker;
    public ObserverBehaviour oxygenTarget;

    private bool isFusing = false;

    void Start()
    {
        if (oxygenTarget != null)
        {
            oxygenTarget.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void Update()
    {
        if (oxygenTarget == null || distanceChecker == null)
            return;

        bool isOxygenDetected = oxygenTarget.TargetStatus.Status == Status.TRACKED;
        bool areHydrogensClose = distanceChecker.AreTwoHydrogensCloseToOxygen();

        if (isOxygenDetected && areHydrogensClose)
        {
            if (!isFusing)
            {
                isFusing = true;
                animator.SetBool("isFusing", true);
                Debug.Log("✅ Fundindo: Oxigênio detectado e dois hidrogênios próximos!");
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

    // Detecta quando o target perde o tracking
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.NO_POSE || status.Status == Status.LIMITED)
        {
            
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
        if (oxygenTarget != null)
        {
            oxygenTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}
