
using UnityEngine;


namespace Game
{

    public enum GAME_STATE
    {
        MENU,
        IN_GAME,
        DEATH
    }

    public class GameManager
    {

        static public bool Debug { get; private set; }
        static public GameObject Player { get; private set; }
        static public GAME_STATE GameState { get; private set; }


        static public void SetDebug(bool value)
        {
            Debug = value;
            DebugDraw.Enabled = value;
        }

        static public void SetPlayer(GameObject player)
        {
            Player = player;
        }

        static public void SetGameState(GAME_STATE state)
        {
            GameState = state;
            Time.timeScale = 1;
        }

    }
}