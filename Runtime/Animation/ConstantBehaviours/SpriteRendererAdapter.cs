using UnityEngine;

[System.Serializable]
public class SpriteRendererAdapter : IUseColorAndSprite
{
    public SpriteRenderer spriteRenderer;
    public Color Color { get => spriteRenderer.color; set => spriteRenderer.color = value; }
    public Sprite Sprite { get => spriteRenderer.sprite; set => spriteRenderer.sprite = value; }
}
