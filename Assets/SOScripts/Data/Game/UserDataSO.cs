using UnityEngine;


namespace Game.SO.Data.Game
{
    [CreateAssetMenu(fileName = "UserData_Data", menuName = "Scriptable Objects/Data/Game/UserDataSO")]
    public class UserDataSO : ScriptableObject
    {

        [field: SerializeField]
        public UserData UserData { get; private set; }


#if UNITY_EDITOR
        private void OnValidate()
        {
            UserData.OnValidate();
        }
#endif
    }
}