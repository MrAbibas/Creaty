using System.IO;
using UnityEngine;

internal static class Cache
{
    private static readonly string CACHE_BASE_PATH = Path.Combine(Application.persistentDataPath, "Cache");
    internal static bool TryLoadImageFromCache(string baseUrl, string imageName, out Sprite sprite)
    {
        sprite = null;
        imageName = imageName.Replace('?', '.').Replace('=', '.');

        if (File.Exists(Path.Combine(CACHE_BASE_PATH, BaseUrlToPath(baseUrl), imageName)) == false)
            return false;

        byte[] bytes = File.ReadAllBytes(Path.Combine(CACHE_BASE_PATH, BaseUrlToPath(baseUrl), imageName));
        Texture2D texture = new Texture2D(350,350);
        texture.LoadImage(bytes);
        sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2);
        return true;
    }
    internal static void SaveImageToCache(string baseUrl, string imageName, Sprite sprite)
    {
        byte[] bytes = null;
        if (imageName.Contains(".png"))
        {
            bytes = sprite.texture.EncodeToPNG();
        }
        else if (imageName.Contains(".jpg"))
        {
            bytes = sprite.texture.EncodeToJPG();
        }
        else if (imageName.Contains(".exr"))
        {
            bytes = sprite.texture.EncodeToEXR();
        }
        else if (imageName.Contains(".tga"))
        {
            bytes = sprite.texture.EncodeToTGA();
        }
        else
            return;

        string path = Path.Combine(CACHE_BASE_PATH, BaseUrlToPath(baseUrl));
        imageName = imageName.Replace('?', '.').Replace('=', '.');
        Directory.CreateDirectory(path);
        File.WriteAllBytes(Path.Combine(path, imageName), bytes);
    }
    private static string BaseUrlToPath(string baseUrl)
    {
        return baseUrl.ToLower().Replace(':', '.').Replace("//", ".").Replace('/', '.').Replace('?', '.');
    }
}
