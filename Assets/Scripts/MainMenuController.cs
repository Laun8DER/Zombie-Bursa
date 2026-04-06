using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const string MainMenuCanvasName = "MainMenu";
    private const string GameplayCanvasName = "Canvas";
    private const string PlayButtonName = "Play_Buttons";
    private const string SettingsButtonName = "Settings_Button";
    private const string CollectionButtonName = "Collection_Buttons";
    private const float StartGameInputDelay = 0.15f;
    private const float MenuFadeOutDuration = 0.25f;
    private const float MenuFadeHoldDuration = 0.05f;
    private const float MenuFadeInDuration = 0.3f;

    private static MainMenuController instance;

    private GameObject mainMenuCanvasObject;
    private GameObject gameplayCanvasObject;
    private Button playButton;
    private PlayerInput playerInput;
    private Coroutine startGameRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            instance.SetupCurrentScene();
            return;
        }

        GameObject controllerObject = new GameObject(nameof(MainMenuController));
        instance = controllerObject.AddComponent<MainMenuController>();
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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SetupCurrentScene();
    }

    private void OnDestroy()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupCurrentScene();
    }

    private void SetupCurrentScene()
    {
        mainMenuCanvasObject = FindSceneObject(MainMenuCanvasName);
        gameplayCanvasObject = FindSceneObject(GameplayCanvasName);
        GameObject playButtonObject = FindSceneObject(PlayButtonName);
        GameObject settingsButtonObject = FindSceneObject(SettingsButtonName);
        GameObject collectionButtonObject = FindSceneObject(CollectionButtonName);
        playerInput = FindFirstObjectByType<PlayerInput>(FindObjectsInactive.Include);

        if (mainMenuCanvasObject == null || gameplayCanvasObject == null || playButtonObject == null)
        {
            mainMenuCanvasObject = null;
            gameplayCanvasObject = null;
            playButton = null;
            playerInput = null;
            Time.timeScale = 1f;
            return;
        }

        EnsureMenuCursorHover(playButtonObject);
        EnsureMenuCursorHover(settingsButtonObject);
        EnsureMenuCursorHover(collectionButtonObject);

        playButton = playButtonObject.GetComponent<Button>();
        if (playButton == null)
        {
            playButton = playButtonObject.AddComponent<Button>();
        }

        Image playButtonImage = playButtonObject.GetComponent<Image>();
        if (playButtonImage != null)
        {
            playButton.targetGraphic = playButtonImage;
        }

        playButton.onClick.RemoveListener(StartGame);
        playButton.onClick.AddListener(StartGame);

        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (mainMenuCanvasObject == null || gameplayCanvasObject == null)
        {
            return;
        }

        mainMenuCanvasObject.SetActive(true);
        gameplayCanvasObject.SetActive(false);

        if (startGameRoutine != null)
        {
            StopCoroutine(startGameRoutine);
            startGameRoutine = null;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null)
        {
            inputManager.ChangeInputMap(PlayerInputManager.InputType.UI);
        }

        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        Time.timeScale = 0f;

        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem != null && playButton != null)
        {
            eventSystem.SetSelectedGameObject(playButton.gameObject);
        }
    }

    private void StartGame()
    {
        if (startGameRoutine != null)
        {
            StopCoroutine(startGameRoutine);
        }

        startGameRoutine = StartCoroutine(StartGameRoutine());
    }

    private System.Collections.IEnumerator StartGameRoutine()
    {
        if (mainMenuCanvasObject == null || gameplayCanvasObject == null)
        {
            yield break;
        }

        if (playButton != null)
        {
            playButton.interactable = false;
        }

        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        yield return ScreenFade.Instance.PlayTransition(
            BeginGameplayState,
            MenuFadeOutDuration,
            MenuFadeHoldDuration,
            MenuFadeInDuration);

        while (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(StartGameInputDelay);

        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null)
        {
            inputManager.ChangeInputMap(PlayerInputManager.InputType.Player);
        }

        if (playerInput != null)
        {
            playerInput.ActivateInput();
            playerInput.SwitchCurrentActionMap("Player");
        }

        if (playButton != null)
        {
            playButton.interactable = true;
        }

        startGameRoutine = null;
    }

    private void BeginGameplayState()
    {
        if (mainMenuCanvasObject != null)
        {
            mainMenuCanvasObject.SetActive(false);
        }

        if (gameplayCanvasObject != null)
        {
            gameplayCanvasObject.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    private static GameObject FindSceneObject(string objectName)
    {
        Transform[] sceneTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Transform sceneTransform in sceneTransforms)
        {
            if (!sceneTransform.gameObject.scene.IsValid())
            {
                continue;
            }

            if (sceneTransform.name == objectName)
            {
                return sceneTransform.gameObject;
            }
        }

        return null;
    }

    private static void EnsureMenuCursorHover(GameObject targetObject)
    {
        if (targetObject == null || targetObject.GetComponent<MenuCursorHover>() != null)
        {
            return;
        }

        targetObject.AddComponent<MenuCursorHover>();
    }
}
