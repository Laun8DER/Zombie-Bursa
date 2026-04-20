using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
public class Timer_Cutscene_Manager : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector director;
    public GameObject TimerZombieDialog;    
    public GameObject templateDialog;
    public GameObject Zombie;

    [Header("Prompt")]
    public GameObject promptObject;
    public float promptFadeInDuration = 0.2f;
    public float promptFadeOutDuration = 0.6f;

    private float promptAlpha = 0f;
    private float targetPromptAlpha = 0f;
    private SpriteRenderer[] promptSprites;

    private bool canInteract = false;

    private void Awake()
    {
        CachePromptComponents();
        SetPromptAlpha(0f);
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager == null || inputManager.actions == null)
        {
            return;
        }

        inputManager.actions.Player.Interact.performed += StartCutscene;
    }

    private void OnDisable()
    {
        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager == null || inputManager.actions == null)
        {
            return;
        }

        inputManager.actions.Player.Interact.performed -= StartCutscene;
    }

    private void Update()
    {
        if (promptObject == null || Mathf.Approximately(promptAlpha, targetPromptAlpha))
            return;

        float duration = targetPromptAlpha > promptAlpha ? promptFadeInDuration : promptFadeOutDuration;
        float fadeSpeed = duration <= 0f ? float.MaxValue : 1f / duration;
        promptAlpha = Mathf.MoveTowards(promptAlpha, targetPromptAlpha, fadeSpeed * Time.deltaTime);
        SetPromptAlpha(promptAlpha);

        if (Mathf.Approximately(promptAlpha, 0f))
            promptObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            HidePrompt();
        }
    }

    private void StartCutscene(InputAction.CallbackContext context)
    {
        Zombie.SetActive(true);
        TimerZombieDialog.SetActive(true);
        if (canInteract && director != null)
        {
            director.Play();
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (promptObject == null) return;

        if (!promptObject.activeSelf)
            promptObject.SetActive(true);

        targetPromptAlpha = 1f;
    }

    private void HidePrompt()
    {
        if (promptObject == null) return;

        targetPromptAlpha = 0f;
    }
    private void CachePromptComponents()
    {
        if (promptObject == null) return;


        promptSprites = promptObject.GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void SetPromptAlpha(float alpha)
    {
        promptAlpha = alpha;
        if (promptSprites != null)
        {
            for (int i = 0; i < promptSprites.Length; i++)
            {
                if (promptSprites[i] == null) continue;

                Color color = promptSprites[i].color;
                color.a = alpha;
                promptSprites[i].color = color;
            }
        }
    }
    public void setTemplateDialogOn()
    {
        templateDialog.SetActive(true);
    }
    public void setTemplateDialogOff()
    {
        templateDialog.SetActive(false);
    }
    public void setZombieDialogOff()
    {
        this.enabled = false;
    }
}
