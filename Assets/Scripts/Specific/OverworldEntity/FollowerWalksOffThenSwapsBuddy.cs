using System.Collections;
using Game;
using Game.SO.Data.Buddy;
using Game.SO.EventChannel;
using UnityEngine;

public class FollowerWalksOffThenSwapsBuddy : MonoBehaviour
{
    [SerializeField] EventChannelSO walkOffChannel;
    [SerializeField] Transform walkOffDestination;
    [SerializeField] BuddyDataSO newBuddy;
    [SerializeField] GameObject standaloneNpcToHide;

    void OnEnable() => walkOffChannel.Subscribe(HandleWalkOff);
    void OnDisable() => walkOffChannel.Unsubscribe(HandleWalkOff);

    void HandleWalkOff() => StartCoroutine(WalkOffThenSwap());

    IEnumerator WalkOffThenSwap()
    {
        EntityOverworldController controller = GameManager.Follower.GetComponent<EntityOverworldController>();
        controller.AIPath.destination = walkOffDestination.position;

        while (!controller.AIPath.reachedEndOfPath)
            yield return null;

        GameManager.CurrentUserData.SetCurrentBuddy(newBuddy);

        if (standaloneNpcToHide)
            standaloneNpcToHide.SetActive(false);
    }
}
