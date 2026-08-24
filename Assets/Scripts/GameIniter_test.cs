using Game;
using Game.SO.Data.Buddy;
using UnityEngine;

public class GameIniter_test : MonoBehaviour
{
    [SerializeField] BuddyDataSO buddyData;

    private void Awake()
    {
        GameManager.CurrentUserData.SetCurrentBuddy(buddyData);

        GameManager.CurrentUserData.PlayerBattleData.Refresh();
        GameManager.CurrentUserData.PlayerBattleData.CurrentHP = GameManager.CurrentUserData.PlayerBattleData.MaxHP;
        GameManager.CurrentUserData.PlayerBattleData.CurrentCurse = GameManager.CurrentUserData.PlayerBattleData.MaxCurse;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Awake();
    }
#endif
}
