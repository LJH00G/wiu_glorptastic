using UnityEngine;

using Game;

[DefaultExecutionOrder(-9999)]
public class GameStateSetter : MonoBehaviour
{
    [SerializeField] GAME_STATE thisGameState;
    private void OnEnable()
    {
        GameManager.SetGameState(thisGameState);
    }
}
