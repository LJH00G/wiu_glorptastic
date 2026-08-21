using Game;
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;

using UnityEngine;
using System.Collections.Generic;

public class ParticleGenerator : MonoBehaviour
{

    [Header("Event Listening Channel")]
    [SerializeField] GenerateParticleEventChannelSO GenerateParticleEventChannel;
    [SerializeField] InstantiateGroupEventChannelSO InstantiateGroupEventChannel;
    [SerializeField] Transform particleGroup = null;
    [SerializeField] string key;

    void HandleGenerateParticleEvent(GenerateParticleEventContext context)
    {
        GameObject generatedP = Instantiate(context.particle, context.pos, context.rot, particleGroup);

        if (context.offsetRot != null)
        {
            generatedP.transform.rotation *= context.offsetRot.Value;
        }
        if (context.offsetPos != null)
            generatedP.transform.position += generatedP.transform.rotation * context.offsetPos.Value;

        context.modification?.Invoke(generatedP.GetComponent<ParticleSystem>());

        Debug.Log($"ParticleGenerator.HandleGenerateParticleEvent() | handled event with {context}");
    }

    private void SetParticleGroup(Dictionary<string,GameObject> go)
    {
        if(go.TryGetValue(key, out GameObject particle))
        {
            particleGroup = particle.transform;
        }
        else
        {
            particleGroup = null;
            Debug.Log("Key mismatch to ParticleGenerator");
        }
    }

    private void OnEnable()
    {
        GenerateParticleEventChannel.Subscribe(HandleGenerateParticleEvent);
        InstantiateGroupEventChannel.Subscribe(SetParticleGroup);
    }

    private void OnDisable()
    {
        GenerateParticleEventChannel.Unsubscribe(HandleGenerateParticleEvent);
        InstantiateGroupEventChannel.Unsubscribe(SetParticleGroup);
    }
}
