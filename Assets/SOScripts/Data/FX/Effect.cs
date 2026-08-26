using Game.SO.EventChannel.Context;
using UnityEngine;

namespace FX
{
    [CreateAssetMenu(fileName = "Effect", menuName = "FX/Effect")]
    public class Effect : ScriptableObject
    {
        public GenerateParticleEventContext particle;
        public PlaySFXEventContext audio;
    }
}

