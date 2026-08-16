
using UnityEngine;

namespace Game.SO.ActionFn.Derived
{
    public abstract class CompoundActionSO<T_ActionSO, P> : ActionSO<P>
        where T_ActionSO : ActionSO<P>
    {
        [SerializeField] T_ActionSO[] actions;

        public override void Invoke(P param)
        {
            foreach (var action in actions)
                action.Invoke(param);
        }
    }

    [CreateAssetMenu(fileName = "Compound_Act", menuName = "Scriptable Objects/ActionFn/CompoundAction/CompoundActionSO")]
    public class CompoundActionSO : ActionSO
    {
        [SerializeField] ActionSO[] actions;

        public override void Invoke()
        {
            foreach (var action in actions)
                action.Invoke();
        }
    }
}