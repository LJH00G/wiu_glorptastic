using UnityEngine;
using Game.SO.EventChannel;
public class TestTP : MonoBehaviour
{
    [SerializeField] StringEventChannelSO TestTPChannel;

    void OnTriggerEnter2D(Collider2D col)
    {
        TestTPChannel.Raise("Test");
        Debug.Log("TP Trigger Occured");
    }

}