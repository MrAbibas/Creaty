using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField urlText;
    [SerializeField]
    private Button goButton;
    [SerializeField]
    private Transform grid;
    [SerializeField]
    private TMP_Text countText;
    [SerializeField]
    private GameObject imagePrefab;
    private Image[] images;
    void Start()
    {
        goButton.onClick.AddListener(() => ImageLoader.Instance.StartLoadImages(urlText.text, ImageCountLoaded, ImageLoaded));
    }
    void ImageCountLoaded(int count)
    {
        countText.text = count.ToString();
        images = new Image[count];
        for (int i = 0; i < count; i++)
            images[i] = Instantiate(imagePrefab, grid).GetComponentInChildren<Image>();
    }
    void ImageLoaded(ImageInfo imageInfo)
    {
        Image image = images[imageInfo.id];
        images[imageInfo.id].sprite = imageInfo.sprite;
        images[imageInfo.id].color = Color.white;
        float x = images[imageInfo.id].rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x;
        images[imageInfo.id].rectTransform.sizeDelta = new Vector2()
        {
            x = x,
            y = image.sprite.texture.height * (x / image.sprite.texture.width)
        };
    }
}
