using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-999999999)]
public class DontDestroyOnLoad : MonoBehaviour
{
    static DontDestroyOnLoad instance;
    public Scene AwokenScene { get; private set; }

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        AwokenScene = gameObject.scene;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
