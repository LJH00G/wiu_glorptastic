using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99999)]
public class ApplicationIniter : MonoBehaviour
{


    private void Start()
    {
        SceneManager.LoadSceneAsync("GeneralUIOverlay", LoadSceneMode.Additive);
    }

}
