using static CellProperties;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTextureData", menuName = "Texture Data", order = 1)]
public class TextureData : ScriptableObject
{
    public CellTypeTexture[] typeTextures = new CellTypeTexture[2]; // Frog ve Berry için texture dizisi
    public Texture2D[] baseTextures = new Texture2D[5];
}
[System.Serializable]
public class CellTypeTexture
{
    public CellType cellType;
    public Texture2D[] textures = new Texture2D[5]; // Her bir renk için texture dizisi (Blue, Green, Purple, Red, Yellow)
}