using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public string sceneNameAR = "SampleScene";
    public string repoURL = "https://quimic-ar-js.vercel.app/";

    public void StartARApp()
    {
        SceneManager.LoadScene(sceneNameAR);
    }

    public void OpenRepository()
    {
        Application.OpenURL(repoURL);
    }

    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("saindo...");
            Application.Quit();
#endif
    }
}
