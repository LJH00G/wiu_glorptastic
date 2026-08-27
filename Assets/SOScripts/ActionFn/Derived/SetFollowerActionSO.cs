using UnityEngine;
using Game.SO.Data.Buddy;

namespace Game.SO.ActionFn
{
    [CreateAssetMenu(fileName = "SetFollower", menuName = "Scriptable Objects/ActionFn/SetFollower")]
    public class SetFollowerActionSO : ActionSO
    {
        [SerializeField] BuddyDataSO buddy;
        public override void Invoke() => GameManager.CurrentUserData.SetCurrentBuddy(buddy);
    }
}