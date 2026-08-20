using Game.SO.EventChannel;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.TPManager
{
    public class TPManager : MonoBehaviour
    {
        [SerializeField] VisualizableDict<string, TPDefinition> TPPointDef;
        [SerializeField] StringEventChannelSO TPCallChannel;
        [SerializeField] BoolEventChannelSO TPAnimChannel;
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

            if (GameManager.Follower != null)
            {

                follower = GameManager.Follower.transform;
                
            }
            else
            {
                Debug.Log("No Follower Reference Exists in GameManager");
            }

            TPAnimChannel.Raise(true);

            yield return new WaitForSeconds(currentTP.time);

            if(player != null)
                player.transform.position = currentTP.position;

            if(follower != null)
                follower.transform.position = currentTP.position;
        

            TPAnimChannel.Raise(false);

            yield return new WaitForSeconds(currentTP.time);

            GameManager.SetPlayerCanMove(true);
        }

        



#if UNITY_EDITOR

        void OnValidate()
        {
            TPPointDef.OnValidate();
        }

#endif
    }

}
