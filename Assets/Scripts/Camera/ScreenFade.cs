using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    private static ScreenFade instance;

    public static ScreenFade Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }

            return instance;
        }
    }

    private CanvasGroup fadeCanvasGroup;
    private GameObject fadeCanvasObject;
    private bool isTransitioning;

    [Header("Custom Animation")]
    public Animator animator;
    public string fadeOutState = "FadeOut";
    public string fadeInState = "FadeIn";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        CreateInstance();
    }

    private static void CreateInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("ScreenFade");
        instance = go.AddComponent<ScreenFade>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureOverlay();
    }

    public IEnumerator PlayTransition(Action midpointAction, float fadeOutDuration, float holdDuration, float fadeInDuration)
    {
        if (isTransitioning)
        {
            yield break;
        }

        EnsureOverlay();
        isTransitioning = true;

        fadeCanvasObject.SetActive(true);
        fadeCanvasGroup.blocksRaycasts = true;

        if (animator != null)
        {
            animator.Play(fadeOutState);
            yield return new WaitForSecondsRealtime(fadeOutDuration);
        }
        else
        {
            yield return Fade(1f, fadeOutDuration);
        }

        midpointAction?.Invoke();

        if (holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(holdDuration);
        }

        if (animator != null)
        {
            animator.Play(fadeInState);
            yield return new WaitForSecondsRealtime(fadeInDuration);
        }
        else
        {
            yield return Fade(0f, fadeInDuration);
        }

        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasObject.SetActive(false);
        isTransitioning = false;
    }

    private void EnsureOverlay()
    {
        if (fadeCanvasGroup != null)
        {
            return;
        }

        fadeCanvasObject = new GameObject("FadeCanvas");
        fadeCanvasObject.transform.SetParent(transform, false);

        Canvas canvas = fadeCanvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = fadeCanvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        fadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvasGroup = fadeCanvasObject.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(fadeCanvasObject.transform, false);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        fadeCanvasObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;

        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
