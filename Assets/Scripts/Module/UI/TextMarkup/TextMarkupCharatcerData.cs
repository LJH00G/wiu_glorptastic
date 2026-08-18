
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.TextMarkup
{

    public struct CharacterVertex
    {
        public Vector2 position;
        public Color32 color;

        public void Set(Vector2 position, Color32 color)
        {
            this.position = position;
            this.color = color;
        }
    }

    public struct CharacterData
    {
        public int materialIndex;
        public int vertexIndex;
        public CharacterVertex[] originalVertices;

        public List<TextMarkupEffect> effects;


        public CharacterData(int materialIndex, int vertexIndex, CharacterVertex[] originalVertices, List<TextMarkupEffect> effects)
        {
            this.materialIndex = materialIndex;
            this.vertexIndex = vertexIndex;
            this.originalVertices = originalVertices;
            this.effects = new(effects);
        }
    }

}