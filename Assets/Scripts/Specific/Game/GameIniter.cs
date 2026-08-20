using Game;
using UnityEngine;

public class GameIniter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject follower;

    private void Awake()
    {
        GameManager.SetPlayer(player);
        GameManager.SetFollower(follower);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
