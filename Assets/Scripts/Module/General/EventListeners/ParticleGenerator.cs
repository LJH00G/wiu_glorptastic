using Game;
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel.Derived;

using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{

    [Header("Event Listening Channel")]
    [SerializeField] GenerateParticleEventChannelSO GenerateParticleEventChannel;

    [SerializeField] Transform particleGroup = null;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GenerateParticleEventChannel.Subscribe(HandleGenerateParticleEvent);
    }

    private void OnDisable()
    {
        GenerateParticleEventChannel.Unsubscribe(HandleGenerateParticleEvent);
    }
}
