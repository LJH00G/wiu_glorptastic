using Game.Combat;
using UnityEngine;


namespace Game.GlobalVariable
{
    [DefaultExecutionOrder(-99999)]
    public class StaticGlobalVariableAssigner : MonoBehaviour
    {
        [SerializeField]
        PlayerLoadoutSO playerLoadout;

        private void Awake()
        {
            StaticGlobalVariable.PlayerLoadout = playerLoadout;
        }
    }

    static public class StaticGlobalVariable
    {
        static public PlayerLoadoutSO PlayerLoadout { get; set; }
    }
}