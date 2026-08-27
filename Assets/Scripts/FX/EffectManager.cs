using Game.GlobalVariable;
using Game.SO.EventChannel;

namespace FX
{
    public static class EffectManager
    {
        public static void RecieveSignal(Effect effect)
        {
<<<<<<< Updated upstream
            StaticGlobalVariable.GenerateParticleEventChannel.Raise(effect.particle);
            StaticGlobalVariable.PlaySFXEventChannel.Raise(effect.audio);
=======
            particleChannel?.Raise(effect.particle);
            playSFXChannel?.Raise(effect.audio);
>>>>>>> Stashed changes
        }

    }
}

