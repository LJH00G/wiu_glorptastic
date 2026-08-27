using Game;
using Game.SO.ActionFn;
using UnityEngine;

[CreateAssetMenu(fileName = "SetFlag", menuName = "Scriptable Objects/ActionFn/SetFlag")]
public class SetFlagActionSO : ActionSO
{
    [SerializeField] string flagKey;
    [SerializeField] bool value = true;
    public override void Invoke()
    {
        GameManager.EnsureFlag(flagKey, value);
        GameManager.SetFlag(flagKey, value);
    }
}
