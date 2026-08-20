using UnityEngine;


namespace Game
{
    /// <summary>
    /// add flags
    /// - can interact
    /// - can move
    /// - functions that forcefully set players position based on where they need or want to go, like teleporting after interacting with a portal of sorts
    /// </summary>
    public enum GAME_STATE
    {
        MENU,
        IN_GAME,
        DEATH
    }

    public class GameManager
    {

        static public bool Debug { get; private set; } = true;
        static public GameObject Player { get; private set; }
        static public GameObject Follower { get; private set; }
        static public bool PlayerCanMove { get; private set; } = true;
        static public bool AllCanMove { get; private set; } = true;
        static public GAME_STATE GameState { get; private set; }
        static public UserData CurrentUserData {get; private set; } = new(); //tracks the currently in use player saved data so scripts like inventory manager can be linked up after load attempt without a scene in use
        static public bool CanInteract { get; private set; } = true;
        static public bool Paused { get; private set; }


        static public void SetDebug(bool value)
        {
            Debug = value;
            DebugDraw.Enabled = value;

            UnityEngine.Debug.Log($"GameManager.Debug: {Debug}");
        }

        static public void SetPlayer(GameObject player)
        {
            Player = player;
        }

        static public void SetFollower(GameObject follower)
        {
            Follower = follower;
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

        static public void SetCanInteract(bool canInteract)
        {
            CanInteract = canInteract;
        }

        static public void SetPause(bool pause)
        {
            Paused = pause;
        }

        static public void SetPlayerCanMove(bool value)
        {
            PlayerCanMove = value;
        }

        static public void SetAllCanMove(bool value)
        {
            AllCanMove = value;
        }
    }
}