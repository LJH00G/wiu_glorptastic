using Game.SO.EventChannel;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utility.VisualizableDictionary;
using Game.SO.EventChannel.Context;


namespace Game.TPManager
{
    public class TPManager : MonoBehaviour
    {
        [SerializeField] VisualizableDict<string, TPDefinition> TPPointDef;
        [SerializeField] StringEventChannelSO TPCallChannel;
        [SerializeField] FadeEventChannelSO TPAnimChannel;
        private float fadeTime;
        


        
        void OnEnable()
        {
            TPCallChannel.Subscribe(PerformTeleport);
            
        }

        void OnDisable()
        {
            TPCallChannel.Unsubscribe(PerformTeleport);
        }

        void PerformTeleport(string TPName)
        {
            if(TPPointDef.dict.TryGetValue(TPName, out TPDefinition currentTP))
            {
                StartCoroutine(Teleport(currentTP));
            }
        }

        public IEnumerator Teleport(TPDefinition currentTP)
        {
            

            Transform player = null;
            Transform follower = null;

            if (GameManager.Player != null)
            {

                player = GameManager.Player.transform;
                GameManager.SetPlayerCanMove(false);
            }
            else
            {
                Debug.Log("No Player Reference Exists in GameManager!");
            }

            if (GameManager.Follower != null && GameManager.Follower.activeInHierarchy)
            {

                follower = GameManager.Follower.transform;

            }
            else
            {
                Debug.Log("No Follower Reference Exists in GameManager");
            }

            FadeEventChannelContext fade = new FadeEventChannelContext(true, currentTP.time);
            TPAnimChannel.Raise(fade);

            yield return new WaitForSeconds(2f / currentTP.time);

            if (player != null)
            {
                Vector3 positionDelta = currentTP.position - (Vector2)player.position;
                player.GetComponent<EntityOverworldController>().Teleport(currentTP.position);
                CinemachineCore.OnTargetObjectWarped(player, positionDelta);
            }
            if (follower != null && GameManager.Follower.activeInHierarchy)
            {
                Vector3 positionDelta = currentTP.position - (Vector2)follower.position;
                follower.GetComponent<EntityOverworldController>().Teleport(currentTP.position);
                CinemachineCore.OnTargetObjectWarped(follower, positionDelta);
            }
            fade.isFade = false;
            TPAnimChannel.Raise(fade);

            yield return new WaitForSeconds(2f / currentTP.time);

            GameManager.SetPlayerCanMove(true);
        }

        



#if UNITY_EDITOR

        void OnValidate()
        {
            TPPointDef.OnValidate();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 0, 1);
            Vector3 Gizmossize = new Vector3(1, 1, 1) * 0.5f;
            foreach(TPDefinition TP in TPPointDef.dict.Values)
            {
                
                Gizmos.DrawWireCube(new Vector3(TP.position.x, TP.position.y, 0), Gizmossize);
            }
        }

#endif
    }

}
