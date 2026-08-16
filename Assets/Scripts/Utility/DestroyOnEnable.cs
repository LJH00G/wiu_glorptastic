using UnityEngine;

public class DestroyOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject);
    }
}
