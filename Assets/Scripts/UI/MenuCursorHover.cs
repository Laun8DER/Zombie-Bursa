using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static Texture2D pointerCursorTexture;
    private static readonly Vector2 PointerHotspot = new Vector2(4f, 2f);

    [SerializeField] private float hoverScaleMultiplier = 1.08f;
    [SerializeField] private float scaleLerpSpeed = 12f;

    private RectTransform rectTransform;
    private Vector3 defaultScale;
    private Vector3 targetScale;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        defaultScale = transform.localScale;
        targetScale = defaultScale;
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleLerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = defaultScale * hoverScaleMultiplier;
        Cursor.SetCursor(GetPointerCursorTexture(), PointerHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = defaultScale;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnDisable()
    {
        targetScale = defaultScale;
        transform.localScale = defaultScale;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private static Texture2D GetPointerCursorTexture()
    {
        if (pointerCursorTexture != null)
        {
            return pointerCursorTexture;
        }

        pointerCursorTexture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        pointerCursorTexture.filterMode = FilterMode.Point;
        pointerCursorTexture.wrapMode = TextureWrapMode.Clamp;

        Color32 transparent = new Color32(0, 0, 0, 0);
        Color32 outline = new Color32(18, 18, 18, 255);
        Color32 fill = new Color32(245, 245, 245, 255);

        Color32[] pixels = new Color32[16 * 16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = transparent;
        }

        SetPixel(pixels, 0, 15, outline);
        SetPixel(pixels, 0, 14, outline);
        SetPixel(pixels, 1, 14, outline);
        SetPixel(pixels, 0, 13, outline);
        SetPixel(pixels, 1, 13, fill);
        SetPixel(pixels, 2, 13, outline);
        SetPixel(pixels, 0, 12, outline);
        SetPixel(pixels, 1, 12, fill);
        SetPixel(pixels, 2, 12, fill);
        SetPixel(pixels, 3, 12, outline);
        SetPixel(pixels, 0, 11, outline);
        SetPixel(pixels, 1, 11, fill);
        SetPixel(pixels, 2, 11, fill);
        SetPixel(pixels, 3, 11, fill);
        SetPixel(pixels, 4, 11, outline);
        SetPixel(pixels, 0, 10, outline);
        SetPixel(pixels, 1, 10, fill);
        SetPixel(pixels, 2, 10, fill);
        SetPixel(pixels, 3, 10, fill);
        SetPixel(pixels, 4, 10, fill);
        SetPixel(pixels, 5, 10, outline);
        SetPixel(pixels, 0, 9, outline);
        SetPixel(pixels, 1, 9, fill);
        SetPixel(pixels, 2, 9, fill);
        SetPixel(pixels, 3, 9, fill);
        SetPixel(pixels, 4, 9, fill);
        SetPixel(pixels, 5, 9, fill);
        SetPixel(pixels, 6, 9, outline);
        SetPixel(pixels, 0, 8, outline);
        SetPixel(pixels, 1, 8, fill);
        SetPixel(pixels, 2, 8, fill);
        SetPixel(pixels, 3, 8, fill);
        SetPixel(pixels, 4, 8, fill);
        SetPixel(pixels, 5, 8, fill);
        SetPixel(pixels, 6, 8, fill);
        SetPixel(pixels, 7, 8, outline);
        SetPixel(pixels, 0, 7, outline);
        SetPixel(pixels, 1, 7, fill);
        SetPixel(pixels, 2, 7, fill);
        SetPixel(pixels, 3, 7, fill);
        SetPixel(pixels, 4, 7, fill);
        SetPixel(pixels, 5, 7, fill);
        SetPixel(pixels, 6, 7, fill);
        SetPixel(pixels, 7, 7, fill);
        SetPixel(pixels, 8, 7, outline);
        SetPixel(pixels, 0, 6, outline);
        SetPixel(pixels, 1, 6, outline);
        SetPixel(pixels, 2, 6, outline);
        SetPixel(pixels, 3, 6, fill);
        SetPixel(pixels, 4, 6, fill);
        SetPixel(pixels, 5, 6, fill);
        SetPixel(pixels, 6, 6, fill);
        SetPixel(pixels, 7, 6, fill);
        SetPixel(pixels, 8, 6, fill);
        SetPixel(pixels, 9, 6, outline);
        SetPixel(pixels, 3, 5, outline);
        SetPixel(pixels, 4, 5, outline);
        SetPixel(pixels, 5, 5, fill);
        SetPixel(pixels, 6, 5, fill);
        SetPixel(pixels, 7, 5, fill);
        SetPixel(pixels, 8, 5, outline);
        SetPixel(pixels, 9, 5, fill);
        SetPixel(pixels, 10, 5, outline);
        SetPixel(pixels, 4, 4, outline);
        SetPixel(pixels, 5, 4, fill);
        SetPixel(pixels, 6, 4, fill);
        SetPixel(pixels, 7, 4, fill);
        SetPixel(pixels, 8, 4, outline);
        SetPixel(pixels, 9, 4, fill);
        SetPixel(pixels, 10, 4, outline);
        SetPixel(pixels, 5, 3, outline);
        SetPixel(pixels, 6, 3, fill);
        SetPixel(pixels, 7, 3, fill);
        SetPixel(pixels, 8, 3, outline);
        SetPixel(pixels, 9, 3, fill);
        SetPixel(pixels, 10, 3, outline);
        SetPixel(pixels, 6, 2, outline);
        SetPixel(pixels, 7, 2, fill);
        SetPixel(pixels, 8, 2, outline);
        SetPixel(pixels, 9, 2, fill);
        SetPixel(pixels, 10, 2, outline);
        SetPixel(pixels, 7, 1, outline);
        SetPixel(pixels, 8, 1, outline);
        SetPixel(pixels, 9, 1, fill);
        SetPixel(pixels, 10, 1, outline);
        SetPixel(pixels, 9, 0, outline);

        pointerCursorTexture.SetPixels32(pixels);
        pointerCursorTexture.Apply();
        return pointerCursorTexture;
    }

    private static void SetPixel(Color32[] pixels, int x, int y, Color32 color)
    {
        pixels[(y * 16) + x] = color;
    }
}
