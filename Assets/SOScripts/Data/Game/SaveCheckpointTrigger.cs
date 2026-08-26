using UnityEngine;
using Game;

[RequireComponent(typeof(BoxCollider2D))]
public class SaveCheckpointTrigger : MonoBehaviour
{
    [SerializeField] string checkpointID;
    public string CheckpointID => checkpointID;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.Player)
        {
            return;
        }
        GameManager.CurrentUserData.SetCheckpoint(checkpointID);
        SaveManager.Save(SaveManager.FromUserData(GameManager.CurrentUserData));

        Debug.Log($"SaveCheckpointTrigger.OnTriggerEnter2D() | saved at checkpoint {checkpointID}");
    }
}