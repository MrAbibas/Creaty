using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class SiteLoader : MonoBehaviour
{
    private static SiteLoader instance;
    internal static SiteLoader Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    internal void StartLoad(string url, UnityAction<string> loadHandler)
    {
        StopAllCoroutines();
        StartCoroutine(Load(url, loadHandler));
    }
    internal IEnumerator Load(string url, UnityAction<string> loadHandler)
    {
        UnityWebRequest data = new UnityWebRequest(url);
        data.downloadHandler = new DownloadHandlerBuffer();
        yield return data.SendWebRequest();
        if(data.isDone && data.result == UnityWebRequest.Result.Success)
        {
            loadHandler?.Invoke(data.downloadHandler.text);
        }
    }
}