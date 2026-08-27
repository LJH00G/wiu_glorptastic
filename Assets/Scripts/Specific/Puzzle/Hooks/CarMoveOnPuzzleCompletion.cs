using Game;
using Game.SO.EventChannel;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class CarMoveOnPuzzleCompletion : MonoBehaviour
{
    [SerializeField] EventChannelSO puzzleSolved;
    [SerializeField] string flagName;
    [SerializeField] Collider2D blockingCollider;
    [SerializeField] Transform anchor;

    void OnEnable() => puzzleSolved.Subscribe(MoveOutOfTheWay);
    void OnDisable() => puzzleSolved.Unsubscribe(MoveOutOfTheWay);

    void Awake()
    {

        if (GameManager.CurrentUserData.Flags.dict.TryGetValue(flagName, out bool moved) && moved)
            SnapToMovedState();
    }

    void MoveOutOfTheWay()
    {
        GameManager.SetFlag(flagName);
        blockingCollider.enabled = false;
        StartCoroutine(SlideToAnchor());
    }

    void SnapToMovedState()
    {
        this.gameObject.transform.position = anchor.transform.position;
        this.gameObject.transform.rotation = anchor.transform.rotation;
    }

    IEnumerator SlideToAnchor()
    {
        Vector3 start = transform.position;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; 
            transform.position = Vector3.Lerp(start, anchor.position, t);
            yield return null;
        }
    }
}

