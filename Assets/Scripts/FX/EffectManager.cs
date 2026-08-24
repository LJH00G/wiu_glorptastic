using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;

namespace FX
{
    public static class EffectManager
    {

        static public GenerateParticleEventChannelSO particleChannel {  get; private set; }
        static public PlaySFXEventChannelSO playSFXChannel { get; private set; }

        public static void SetParticleEventChannel(GenerateParticleEventChannelSO newParticleChannel)
        {
            particleChannel = newParticleChannel;
        }

        public static void SetSFXEventChannel(PlaySFXEventChannelSO newSFXChannel)
        {
            playSFXChannel = newSFXChannel;
        }

        public static void RecieveSignal(Effect effect)
        {
            particleChannel.Raise(effect.particle);
            playSFXChannel.Raise(effect.audio);
        }

    }
}

