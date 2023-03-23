using UnityEngine.Networking;
using HtmlAgilityPack;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System.Linq;

internal class ImageLoader: MonoBehaviour
{
    private static ImageLoader instance;
    internal static ImageLoader Instance => instance;

    static string baseUrl;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    internal void StartLoadImages(string url, UnityAction<int> countImageLoadHandler, UnityAction<ImageInfo> imageLoadedHandler)
    {
        StopAllCoroutines();
        ImageLoader.baseUrl = url;
        SiteLoader.Instance.StartLoad(url, (html) => LoadImages(html, countImageLoadHandler, imageLoadedHandler));
    }
    internal void LoadImages(string html, UnityAction<int> countImageLoadHandler, UnityAction<ImageInfo> imageLoadedHandler)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        string[] imageUrls = ParseImageUrl(doc);
        countImageLoadHandler?.Invoke(imageUrls.Length);
        for (int i = 0; i < imageUrls.Length; i++)
        {
            StartCoroutine(LoadImage(imageUrls[i], i, imageLoadedHandler));
        }
    }

    internal IEnumerator LoadImage(string url, int id, UnityAction<ImageInfo> imageLoadedHandler)
    {
        string imageName = url.Split('/').Last();
        Sprite sprite = null;
        if (Cache.TryLoadImageFromCache(baseUrl, imageName, out sprite) == false)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            request.downloadHandler = new DownloadHandlerTexture();
            yield return request.SendWebRequest();
            if (request.isDone && request.result != UnityWebRequest.Result.Success)
                yield break;

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            Cache.SaveImageToCache(baseUrl, imageName, sprite);
        }
        ImageInfo imageInfo = new ImageInfo()
        {
            id = id,
            url = url,
            urlBase = ImageLoader.baseUrl,
            sprite = sprite
        };
        imageLoadedHandler?.Invoke(imageInfo);
    }
    internal static string[] ParseImageUrl(HtmlDocument doc)
    {
        List<string> urls = new List<string>();
        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//img"))
        {
            HtmlAttribute att = link.Attributes["src"];
            if (att == null || string.IsNullOrEmpty(att.Value)) continue;
            if (att.Value.Contains(".svg")) continue;
            if (att.Value[0] == '/')
                urls.Add(baseUrl + att.Value);
            else
                urls.Add(att.Value);
        }
        return urls.ToArray();
    }
}
