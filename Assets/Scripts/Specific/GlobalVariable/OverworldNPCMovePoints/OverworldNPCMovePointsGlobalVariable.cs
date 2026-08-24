
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.GlobalVariable.OverworldNPCMovePoints
{
    [Serializable]
    public struct MovePoints
    {
        public Vector2[] points;
        public Color debugColor;
    }

    static public class OverworldNPCMovePointsGlobalVariable
    {
        static public Dictionary<string, MovePoints> MovePointsDict { get; set; } = new();
    }
}