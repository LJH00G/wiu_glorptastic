using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLayerByPosition : MonoBehaviour
{
    
    [SerializeField, Tooltip("in worldspace unit")] float offset;

    public const int PRECISION = 100;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = -(int)((transform.position.y + offset) * PRECISION);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;

        Vector2 referencePoint = transform.position;
        referencePoint.y += offset;
        Gizmos.DrawLine(referencePoint - Vector2.right, referencePoint + Vector2.right);
    }
#endif
}
