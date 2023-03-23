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
    private Vector2 gridCellSize;
    void Start()
    {
        gridCellSize = grid.GetComponent<GridLayoutGroup>().cellSize;
        goButton.onClick.AddListener(GoButtonClick);
    }
    void GoButtonClick()
    {
        ClearImages();
        ImageLoader.Instance.StartLoadImages(urlText.text, ImageCountLoaded, ImageLoaded);
    }
    void ClearImages()
    {
        if (images == null || images.Length == 0) return;
        for (int i = 0; i < images.Length; i++)
        {
            Destroy(images[i].rectTransform.parent.gameObject);
        }
        images = null;
        countText.text = "0";
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
        image.sprite = imageInfo.sprite;
        image.color = Color.white;
        images[imageInfo.id].rectTransform.sizeDelta = new Vector2()
        {
            x = gridCellSize.x,
            y = Mathf.Min(gridCellSize.x * image.sprite.texture.height/image.sprite.texture.width, gridCellSize.y),
        };
    }
}
