using Game.GlobalVariable;
using Game.SO.EventChannel;

namespace FX
{
    public static class EffectManager
    {
        public static void RecieveSignal(Effect effect)
        {
            StaticGlobalVariable.GenerateParticleEventChannel.Raise(effect.particle);
            StaticGlobalVariable.PlaySFXEventChannel.Raise(effect.audio);
        }

    }
}

