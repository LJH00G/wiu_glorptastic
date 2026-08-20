using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PipeSpriteConfig", menuName = "PuzzleData/Pipe/SpriteConfig")]
public class PipeSpriteConfig : ScriptableObject
{
    [System.Serializable]
    public struct Entry 
    { 
        public string pipeName; 
        public Sprite sprite; 
    }
    public List<Entry> entries;

    public Sprite GetSprite(string pipeName)
    {

        foreach (var entry in entries)
        {
            if (entry.pipeName == pipeName)
                return entry.sprite;
        }

        Debug.LogError($"[PipeSpriteConfig] No sprite entry found for pipeName '{pipeName}'. " +
                        $"Available entries: {string.Join(", ", entries.Select(e => $"'{e.pipeName}'"))}");
        return null;
    }
} 

