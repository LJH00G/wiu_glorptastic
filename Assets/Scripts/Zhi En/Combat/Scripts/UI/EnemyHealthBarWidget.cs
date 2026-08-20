using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// the little health bar that hovers below an enemy - a number on the left, a fill bar on
    /// the right. Spawned once per enemy at battle start and updated in place (enemies don't
    /// move, so no per-frame position tracking needed).
    /// </summary>
    public class EnemyHealthBarWidget : MonoBehaviour
    {
        [SerializeField] Image fillImage;
        [SerializeField] TextMeshProUGUI numberText;

        public void SetValue(int current, int max)
        {
            if (numberText) numberText.text = current.ToString();
            if (fillImage) fillImage.fillAmount = max <= 0 ? 0f : Mathf.Clamp01((float)current / max);
        }
    }
}
