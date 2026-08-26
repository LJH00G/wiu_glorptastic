using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PositionTileScroller : MonoBehaviour
{
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 speed;
    [SerializeField] Vector2 repeatingTileSize;
    [SerializeField, DisplayOnly] Vector2 repeatingTileSize_worldspace;

    SpriteRenderer spriteRenderer;
    Sprite prevSprite;


    void UpdateTileSize()
    {
        float unitPerPixel = 1 / spriteRenderer.sprite.pixelsPerUnit;
        repeatingTileSize_worldspace = repeatingTileSize * unitPerPixel;
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        prevSprite = spriteRenderer.sprite;

        transform.position = startPos;

        if (spriteRenderer.sprite)
            UpdateTileSize();
    }


    private void Update()
    {
        float dt = Time.deltaTime;


        if (prevSprite != spriteRenderer.sprite && spriteRenderer.sprite)
        {
            prevSprite = spriteRenderer.sprite;
            UpdateTileSize();
        }


        Vector2 pos = transform.localPosition;

        pos.x += dt * speed.x;
        pos.y += dt * speed.y;

        float actualTileSizeX = repeatingTileSize_worldspace.x * transform.localScale.x;
        float actualTileSizeY = repeatingTileSize_worldspace.y * transform.localScale.y;

        while (Mathf.Abs(pos.x - startPos.x) >= actualTileSizeX)
            pos.x += actualTileSizeX * (speed.x > 0 ? -1 : 1);
        while (Mathf.Abs(pos.y - startPos.y) >= actualTileSizeY)
            pos.y += actualTileSizeY * (speed.y > 0 ? -1 : 1);

        transform.localPosition = pos;
    }
}
