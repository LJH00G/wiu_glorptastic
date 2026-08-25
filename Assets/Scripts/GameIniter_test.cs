using Game;
using Game.SO.Data.Buddy;
using UnityEngine;

public class GameIniter_test : MonoBehaviour
{
    [SerializeField] BuddyDataSO buddyData;

    private void Awake()
    {
        GameManager.CurrentUserData.SetCurrentBuddy(buddyData);
        Debug.LogWarning("settled test initer");
        Debug.Log(GameManager.CurrentUserData.CurrentEquipedBuddy, this);

        GameManager.CurrentUserData.PlayerBattleData.Refresh();
        GameManager.CurrentUserData.PlayerBattleData.CurrentHP = GameManager.CurrentUserData.PlayerBattleData.MaxHP;
        GameManager.CurrentUserData.PlayerBattleData.CurrentCurse = GameManager.CurrentUserData.PlayerBattleData.MaxCurse;
    }


    private void Update()
    {
        Debug.Log(GameManager.CurrentUserData.CurrentEquipedBuddy, this);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        Awake();
    }
#endif
}
