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
        static public UserData CurrentUserData {get; private set; } //tracks the currently in use player saved data so scripts like inventory manager can be linked up after load attempt without a scene in use
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

        //called once while the save file is loaded right alongside SetPlayer() in the same step
        static public void SetUserData(UserData userData)
        {
            CurrentUserData = userData;
        }
    }
}