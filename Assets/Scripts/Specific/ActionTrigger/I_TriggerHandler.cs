

namespace Game.TriggerHandler
{
    
    public interface I_TriggerHandler
    {
        public bool RequiresInteraction();
        public void Trigger();
    }
}