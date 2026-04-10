using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }
    public PlayerInputSystem actions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Init()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("InputManager");
        Instance = go.AddComponent<PlayerInputManager>();
        DontDestroyOnLoad(go);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        actions = new PlayerInputSystem();
        actions.Enable();

    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            actions.Disable();
            actions = null;
            Instance = null;
        }
    }
    public void ChangeInputMap(InputType inputType)
    {
        Debug.Log($"ChangeInputMap: {inputType} | {System.Environment.StackTrace}");
        switch (inputType)
        {
            case InputType.Player:
                actions.Player.Enable();
                actions.UI.Disable();
                actions.Cutscenes.Disable();
                break;
            case InputType.UI:
                actions.UI.Enable();
                actions.Player.Disable();
                actions.Cutscenes.Disable();
                break;
            case InputType.Cutscenes:
                actions.Cutscenes.Enable(); 
                actions.Player.Disable();   
                actions.UI.Disable();
                break;
        }
    }
    public enum InputType
    {
        Player, UI, Cutscenes
    }
}