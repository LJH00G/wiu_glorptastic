using System;
using UnityEngine;
using Game.SO.Data.Item;
using Game.Inventory;

namespace Game.SO.ActionFn
{
    [CreateAssetMenu(fileName = "SetFollower", menuName = "Scriptable Objects/ActionFn/SetFollower")]
    public class SetFollowerActionSO : ActionSO
    {
        public GameObject follower;
        public Vector3 location;
        public override void Invoke()
        {
            follower.transform.position = location;

            Instantiate(follower);

            GameManager.SetFollower(follower);


        }
    }
    
}