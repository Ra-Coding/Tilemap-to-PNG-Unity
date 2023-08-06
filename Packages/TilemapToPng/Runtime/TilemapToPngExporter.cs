using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapToPngExporter : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private int _minX, _maxX, _minY, _maxY;
    private Texture2D _image;

    public Texture2D Image
    {
        get
        {
            return _image;
        }
    }

    public void Pack()
    {
        _tilemap = GetComponent<Tilemap>();
        Sprite anySprite = null;

        // Find the minimum and maximum points
        for (int x = 0; x < _tilemap.size.x; x++)
        {
            for (int y = 0; y < _tilemap.size.y; y++)
            {
                Vector3Int pos = new(-x, -y, 0);
                if (_tilemap.GetSprite(pos) != null)
                {
                    // Select any sprite to determine dimensions later
                    anySprite = _tilemap.GetSprite(pos);
                    if (_minX > pos.x)
                    {
                        _minX = pos.x;
                    }
                    if (_minY > pos.y)
                    {
                        _minY = pos.y;
                    }
                }

                pos = new Vector3Int(x, y, 0);
                if (_tilemap.GetSprite(pos) != null)
                {
                    if (_maxX < pos.x)
                    {
                        _maxX = pos.x;
                    }
                    if (_maxY < pos.y)
                    {
                        _maxY = pos.y;
                    }
                }
            }
        }

        // Determine sprite size in pixels
        float width = anySprite.rect.width;
        float height = anySprite.rect.height;

        // Create a texture with size multiplied by the number of cells
        Texture2D createdImage = new((int)width * _tilemap.size.x, (int)height * _tilemap.size.y);

        // Assign entire image as invisible
        Color[] transparent = new Color[createdImage.width * createdImage.height];
        for (int i = 0; i < transparent.Length; i++)
        {
            transparent[i] = new Color(0f, 0f, 0f, 0f);
        }
        createdImage.SetPixels(0, 0, createdImage.width, createdImage.height, transparent);

        // Assign respective pixels to each block
        for (int x = _minX; x <= _maxX; x++)
        {
            for (int y = _minY; y <= _maxY; y++)
            {
                if (_tilemap.GetSprite(new Vector3Int(x, y, 0)) != null)
                {
                    // Map pixels so that minX = 0 and minY = 0
                    createdImage.SetPixels((x - _minX) * (int)width, (y - _minY) * (int)height, (int)width, (int)height,
                        GetCurrentSprite(_tilemap.GetSprite(new Vector3Int(x, y, 0))).GetPixels());
                }
            }
        }
        createdImage.Apply();

        // Store the ready image texture
        _image = createdImage;
    }

    // Method to obtain the cropped sprite as configured
    Texture2D GetCurrentSprite(Sprite sprite)
    {
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                         (int)sprite.textureRect.y,
                                         (int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);
        Texture2D texture = new((int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

#if UNITY_EDITOR
    // Method to export as PNG
    public void ExportAsPng(string name)
    {
        byte[] bytes = _image.EncodeToPNG();
        var dirPath = Application.dataPath + "/Exported Tilemaps/";

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        File.WriteAllBytes(dirPath + name + ".png", bytes);
        AssetDatabase.Refresh();
        _image = null;
    }
#endif
}