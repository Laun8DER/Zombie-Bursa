using UnityEngine;
using UnityEngine.UI;

public class PromptFade : MonoBehaviour
{
    [Header("Prompt settings")]
    public GameObject promptObject;          // сама кнопка або іконка "E"
    public float fadeInDuration = 0.2f;      // час появи
    public float fadeOutDuration = 0.6f;     // час зникнення
    public string requiredTag = "Player";    // тег об'єкта, який може взаємодіяти

    private float promptAlpha = 0f;
    private float targetAlpha = 0f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    private void Awake()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
            spriteRenderers = promptObject.GetComponentsInChildren<SpriteRenderer>(true);
            originalColors = new Color[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
                Color c = originalColors[i];
                c.a = 0f;
                spriteRenderers[i].color = c;
            }
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(promptAlpha, targetAlpha)) return;

        float duration = targetAlpha > promptAlpha ? fadeInDuration : fadeOutDuration;
        float speed = duration > 0f ? 1f / duration : float.MaxValue;

        promptAlpha = Mathf.MoveTowards(promptAlpha, targetAlpha, speed * Time.deltaTime);
        SetPromptAlpha(promptAlpha);

        if (promptAlpha <= 0f && promptObject.activeSelf)
            promptObject.SetActive(false);
    }

    private void SetPromptAlpha(float alpha)
    {
        if (spriteRenderers == null) return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color c = originalColors[i];
            c.a = originalColors[i].a * alpha;
            spriteRenderers[i].color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(requiredTag))
        {
            ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(requiredTag))
        {
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (promptObject != null && !promptObject.activeSelf)
            promptObject.SetActive(true);

        targetAlpha = 1f;
    }

    private void HidePrompt()
    {
        targetAlpha = 0f;
    }
}