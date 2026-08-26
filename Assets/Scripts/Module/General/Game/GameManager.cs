using Unity.VisualScripting;
using UnityEngine;


namespace Game
{
    public enum GAME_STATE
    {
        MAIN_MENU,
        OVERWORLD,
        BATTLE,
        DEATH,
        ENDING
    }

    public enum OVERWORLD_STATE
    {
        GENERAL,
        PUZZLE,
        CUTSCENE,
        TELEPORT,
        INVENTORY_SHOP,

    }

    /// <summary>
    /// add flags
    /// - can interact
    /// - can move
    /// - functions that forcefully set players position based on where they need or want to go, like teleporting after interacting with a portal of sorts
    /// </summary>
    public class GameManager
    {
        static public bool Debug { get; private set; } = true;
        static public GameObject Player { get; private set; }
        static public GameObject Follower { get; private set; }
        static public bool PlayerCanMove { get; private set; } = true;
        static public bool AllCanMove { get; private set; } = true;
        static public GAME_STATE GameState { get; private set; }
        static public OVERWORLD_STATE OverworldState { get; private set; }
        static public UserData CurrentUserData {get; private set; } = new(); //tracks the currently in use player saved data so scripts like inventory manager can be linked up after load attempt without a scene in use
        static public bool CanInteract { get; private set; } = true;
        static public bool Paused { get; private set; } = false;
        static public EntityOverworldController ConversationPartner { get; private set; }

        static public void SetDebug(bool value)
        {
            Debug = value;
            DebugDraw.Enabled = value;

            UnityEngine.Debug.Log($"GameManager.Debug: {Debug}");
        }

        static public void SetPlayer(GameObject player)
        {
            if (!player.TryGetComponent<EntityOverworldController>(out _))
            {
                UnityEngine.Debug.LogWarning("GameManager.SetPlayer() | cannot set a game object without EntityOverworldController as player");
                return;
            }

            Player = player;
        }

        static public void SetFollower(GameObject follower)
        {
            if (!follower.TryGetComponent<EntityOverworldController>(out _))
            {
                UnityEngine.Debug.LogWarning("GameManager.SetFollower() | cannot set a game object without EntityOverworldController as follower");
                return;
            }

            Follower = follower;
        }
        static public void SetGameState(GAME_STATE state)
        {
            GameState = state;
            Time.timeScale = 1;
        }

        static public void SetOverWorldState(OVERWORLD_STATE state)
        {
            OverworldState = state;
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
            UnityEngine.Debug.Log($"GameManager: seting player can move, value({value})");

            PlayerCanMove = value;

            if (PlayerCanMove)
                Player.GetComponent<EntityOverworldController>().RefreshMovement();
        }

        static public void SetAllCanMove(bool value)
        {
            AllCanMove = value;

            if (AllCanMove)
                Player.GetComponent<EntityOverworldController>().RefreshMovement();
        }
        static public void StartConversation(EntityOverworldController partner)
        {
            var playerController = Player.GetComponent<EntityOverworldController>();
            ConversationPartner = partner;

            playerController.SetFrozen(true, ConversationPartner ? ConversationPartner.transform : null);
            if (ConversationPartner)
                ConversationPartner.SetFrozen(true, playerController.transform);

            SetPlayerCanMove(false);
            SetCanInteract(false);
        }

        static public void EndConversation()
        {
            Player.GetComponent<EntityOverworldController>().SetFrozen(false);

            if (ConversationPartner)
                ConversationPartner.SetFrozen(false);
            ConversationPartner = null;

            SetPlayerCanMove(true);
            SetCanInteract(true);
        }
    }
}