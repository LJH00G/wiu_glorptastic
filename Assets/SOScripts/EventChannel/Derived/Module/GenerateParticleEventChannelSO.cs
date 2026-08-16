using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public struct GenerateParticleEventContext
    {
        public GameObject particle;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3? offsetPos;
        public Quaternion? offsetRot;
        public Action<ParticleSystem> modification;

        public GenerateParticleEventContext(GameObject particle, Vector3 pos, Quaternion rot, Vector3? offsetPos = null, Quaternion? offsetRot = null, Action<ParticleSystem> modification = null)
        {
            this.particle = particle;
            this.pos = pos;
            this.rot = rot;
            this.offsetPos = offsetPos;
            this.offsetRot = offsetRot;
            this.modification = modification;
        }

        public override string ToString()
        {
            return $"GenerateParticleEventContext: particle({particle.name}), pos({pos}), rot({rot}), offsetPos?({offsetPos}) offsetRot?({offsetRot}), modification?({modification}) ";
        }
    }
}

namespace Game.SO.EventChannel.Derived
{
    [CreateAssetMenu(fileName = "GenerateParticleEvent_Channel", menuName = "Scriptable Objects/EventChannel/GenerateParticleEventChannelSO")]
    public class GenerateParticleEventChannelSO : EventChannelSO<GenerateParticleEventContext>
    {
        
    }
}