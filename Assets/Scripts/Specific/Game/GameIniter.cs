using Game;
using UnityEngine;

public class GameIniter : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Awake()
    {
        GameManager.SetPlayer(player);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
