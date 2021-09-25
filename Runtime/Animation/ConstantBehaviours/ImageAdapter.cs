using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ImageAdapter : IUseColorAndSprite
{
    public Image image;
    public Color Color { get => image.color; set => image.color = value; }
    public Sprite Sprite { get => image.sprite; set => image.sprite = value; }
}
