using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using Unity.Cinemachine;

[DisallowMultipleComponent]
public class DoorTeleport : MonoBehaviour
{
    public Transform teleportTarget;
    public string requiredTag = "Player";
    public GameObject promptObject;
    public float promptFadeInDuration = 0.2f;
    public float promptFadeOutDuration = 0.6f;
    public float transitionFadeOutDuration = 0.2f;
    public float transitionHoldDuration = 0.05f;
    public float transitionFadeInDuration = 0.2f;
    public Collider2D targetCameraBounds;

    private SpriteRenderer[] promptSprites;
    private TextMesh[] promptTexts;
    private Graphic[] promptGraphics;
    private Color[] promptSpriteColors;
    private Color[] promptTextColors;
    private Color[] promptGraphicColors;
    private float promptAlpha;
    private float targetPromptAlpha;
    private bool isTeleporting;

    public bool CanInteract(Transform interactor)
    {
        if (teleportTarget == null || interactor == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(requiredTag) && !interactor.CompareTag(requiredTag))
        {
            return false;
        }
        
        return true;
    }

    public void Interact(Transform interactor, Rigidbody2D interactorRb)
    {
        if (teleportTarget == null || interactor == null || isTeleporting)
        {
            return;
        }

        StartCoroutine(TeleportWithFade(interactor, interactorRb));
    }

    private void Awake()
    {
        CachePromptComponents();
        SetPromptAlpha(0f);

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (promptObject == null || Mathf.Approximately(promptAlpha, targetPromptAlpha))
        {
            return;
        }

        float duration = targetPromptAlpha > promptAlpha ? promptFadeInDuration : promptFadeOutDuration;
        float fadeSpeed = duration <= 0f ? float.MaxValue : 1f / duration;
        promptAlpha = Mathf.MoveTowards(promptAlpha, targetPromptAlpha, fadeSpeed * Time.deltaTime);
        SetPromptAlpha(promptAlpha);

        if (Mathf.Approximately(promptAlpha, 0f))
        {
            promptObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsMatchingInteractor(other))
        {
            return;
        }

        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsMatchingInteractor(other))
        {
            return;
        }

        HidePrompt();
    }

    private void ShowPrompt()
    {
        if (promptObject == null)
        {
            return;
        }

        if (!promptObject.activeSelf)
        {
            promptObject.SetActive(true);
        }

        targetPromptAlpha = 1f;
    }

    private void HidePrompt()
    {
        if (promptObject == null)
        {
            return;
        }

        targetPromptAlpha = 0f;
    }

    private void CachePromptComponents()
    {
        if (promptObject == null)
        {
            return;
        }

        promptSprites = promptObject.GetComponentsInChildren<SpriteRenderer>(true);
        promptTexts = promptObject.GetComponentsInChildren<TextMesh>(true);
        promptGraphics = promptObject.GetComponentsInChildren<Graphic>(true);

        promptSpriteColors = new Color[promptSprites.Length];
        for (int i = 0; i < promptSprites.Length; i++)
        {
            promptSpriteColors[i] = promptSprites[i].color;
        }

        promptTextColors = new Color[promptTexts.Length];
        for (int i = 0; i < promptTexts.Length; i++)
        {
            promptTextColors[i] = promptTexts[i].color;
        }

        promptGraphicColors = new Color[promptGraphics.Length];
        for (int i = 0; i < promptGraphics.Length; i++)
        {
            promptGraphicColors[i] = promptGraphics[i].color;
        }
    }

    private bool IsMatchingInteractor(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(requiredTag))
        {
            return true;
        }

        return other.CompareTag(requiredTag);
    }

    private void SetPromptAlpha(float alpha)
    {
        promptAlpha = alpha;

        if (promptSprites != null)
        {
            for (int i = 0; i < promptSprites.Length; i++)
            {
                if (promptSprites[i] == null)
                {
                    continue;
                }

                Color color = promptSpriteColors[i];
                color.a *= alpha;
                promptSprites[i].color = color;
            }
        }

        if (promptTexts != null)
        {
            for (int i = 0; i < promptTexts.Length; i++)
            {
                if (promptTexts[i] == null)
                {
                    continue;
                }

                Color color = promptTextColors[i];
                color.a *= alpha;
                promptTexts[i].color = color;
            }
        }

        if (promptGraphics != null)
        {
            for (int i = 0; i < promptGraphics.Length; i++)
            {
                if (promptGraphics[i] == null)
                {
                    continue;
                }

                Color color = promptGraphicColors[i];
                color.a *= alpha;
                promptGraphics[i].color = color;
            }
        }
    }

    private IEnumerator TeleportWithFade(Transform interactor, Rigidbody2D interactorRb)
    {
        isTeleporting = true;
        HidePrompt();

        ScreenFade screenFade = ScreenFade.Instance;
        if (screenFade == null)
        {
            TeleportNow(interactor, interactorRb);
            isTeleporting = false;
            yield break;
        }

        yield return screenFade.PlayTransition(
            () =>
            {
                SwitchCameraBounds();
                TeleportNow(interactor, interactorRb);
            },
            transitionFadeOutDuration,
            transitionHoldDuration,
            transitionFadeInDuration);

        isTeleporting = false;
    }

    private void TeleportNow(Transform interactor, Rigidbody2D interactorRb)
    {
        Vector3 startPosition = interactor.position;

        if (interactorRb != null)
        {
            interactorRb.linearVelocity = Vector2.zero;
        }

        Vector3 targetPosition = teleportTarget.position;
        targetPosition.z = interactor.position.z;
        interactor.position = targetPosition;

        NotifyCameraWarp(interactor, targetPosition - startPosition);
    }

    private void SwitchCameraBounds()
    {
        if (targetCameraBounds == null)
        {
            return;
        }

        CinemachineConfiner2D confiner = FindActiveConfiner();
        if (confiner == null)
        {
            return;
        }

        confiner.BoundingShape2D = targetCameraBounds;
        InvalidateConfinerCache(confiner);
    }

    private CinemachineConfiner2D FindActiveConfiner()
    {
        for (int i = 0; i < CinemachineBrain.ActiveBrainCount; i++)
        {
            CinemachineBrain brain = CinemachineBrain.GetActiveBrain(i);
            if (brain == null)
            {
                continue;
            }

            CinemachineVirtualCameraBase liveCamera = brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
            if (liveCamera == null)
            {
                continue;
            }

            CinemachineConfiner2D confiner = liveCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                return confiner;
            }
        }

        CinemachineCamera fallbackCamera = FindAnyObjectByType<CinemachineCamera>();
        if (fallbackCamera == null)
        {
            return null;
        }

        return fallbackCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void InvalidateConfinerCache(CinemachineConfiner2D confiner)
    {
        MethodInfo invalidateBoundingShapeCacheMethod = typeof(CinemachineConfiner2D).GetMethod(
            "InvalidateBoundingShapeCache",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (invalidateBoundingShapeCacheMethod != null)
        {
            invalidateBoundingShapeCacheMethod.Invoke(confiner, null);
        }

        MethodInfo invalidateCacheMethod = typeof(CinemachineConfiner2D).GetMethod(
            "InvalidateCache",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (invalidateCacheMethod != null)
        {
            invalidateCacheMethod.Invoke(confiner, null);
        }
    }

    private void NotifyCameraWarp(Transform interactor, Vector3 positionDelta)
    {
        if (interactor == null || positionDelta == Vector3.zero)
        {
            return;
        }

        for (int i = 0; i < CinemachineCore.VirtualCameraCount; ++i)
        {
            var camera = CinemachineCore.GetVirtualCamera(i);
            if (camera == null || camera.Follow != interactor)
            {
                continue;
            }

            camera.OnTargetObjectWarped(interactor, positionDelta);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (teleportTarget == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, teleportTarget.position);
        Gizmos.DrawWireSphere(teleportTarget.position, 0.25f);
    }
}
