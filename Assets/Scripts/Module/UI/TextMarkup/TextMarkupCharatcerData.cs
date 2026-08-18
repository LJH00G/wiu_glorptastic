
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
        public bool isVisible;
        public bool show;

        public int materialIndex;
        public int vertexIndex;
        public CharacterVertex[] originalVertices;

        public List<TextMarkupEffect> effects;


        public CharacterData(int materialIndex, int vertexIndex, CharacterVertex[] originalVertices, List<TextMarkupEffect> effects)
        {
            isVisible = true;
            this.materialIndex = materialIndex;
            this.vertexIndex = vertexIndex;
            this.originalVertices = originalVertices;
            this.effects = effects;
            show = false;
        }

        public CharacterData(bool _ = false)
        {
            isVisible = false;
            show = false;
            materialIndex = vertexIndex = 0;
            originalVertices = null;
            effects = null;
        }
    }

}