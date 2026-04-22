using System;
using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    private static ScreenFade _instance;

    public static ScreenFade Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScreenFade>();
                if (_instance == null)
                {
                    Debug.LogError("ScreenFade not found in the scene!");
                }
            }
            return _instance;
        }
    }

    [Header("Components")]
    public Animator animator;

    [Header("Animation Settings")]
    [Tooltip("The name of the single animation state that handles the whole transition")]
    public string transitionState = "RoomTransition";

    private bool _isTransitioning;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (animator != null)
        {
            animator.gameObject.SetActive(false);
        }
    }

    public IEnumerator PlayTransition(Action midpointAction, float fadeToBlackDuration, float holdDuration, float fadeToClearDuration)
    {
        if (_isTransitioning) yield break;
        if (animator == null)
        {
            midpointAction?.Invoke();
            yield break;
        }

        _isTransitioning = true;
        animator.gameObject.SetActive(true);

        // 1. Запускаем одну общую анимацию
        animator.Play(transitionState, 0, 0f);

        // 2. Ждем, пока экран станет черным (в вашей анимации это 0.15с)
        yield return new WaitForSecondsRealtime(fadeToBlackDuration);

        // 3. Делаем телепорт
        midpointAction?.Invoke();

        // 4. Ждем остаток анимации (в сумме с fadeToClearDuration)
        if (holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(holdDuration);
        }

        yield return new WaitForSecondsRealtime(fadeToClearDuration);

        animator.gameObject.SetActive(false);
        _isTransitioning = false;
    }
}
