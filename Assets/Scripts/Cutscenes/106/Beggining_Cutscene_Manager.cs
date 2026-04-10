using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Beggining_Cutscene_Manager : MonoBehaviour
{
    public PlayerInput playerInput; 
    public Collider2D playerCollider;
    public Rigidbody2D playerRigidBody;
    public GameObject hearts;
    public PlayableDirector WakeUpCutscene;
    public PlayableDirector SleepCutscene;
    public GameObject wakeUpButton;
    public PlayerInputHandler playerInputHandler;
    public Timer_Cutscene_Manager timerCutscene;


    private void Start()
    {
        SetPlayerOff();
    }
    public void OnEnable()
    {
        if (PlayerInputManager.Instance == null) return;
        PlayerInputManager.Instance.actions.Cutscenes.Interact.performed += startCutscene;
    }
    public void OnDisable()
    {
        if (PlayerInputManager.Instance == null) return;
        PlayerInputManager.Instance.actions.Cutscenes.Interact.performed -= startCutscene;
    }
    private void startCutscene(InputAction.CallbackContext context)
    {
        wakeUpButton.SetActive(false);
        SleepCutscene.Stop();
        WakeUpCutscene.Play();
    }
    public void SetPlayerOff()
    {
        playerInputHandler.enabled = false;
        playerCollider.enabled = false;
        playerRigidBody.simulated = false;
        hearts.SetActive(false);


        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null)
        {
            inputManager.ChangeInputMap(PlayerInputManager.InputType.Cutscenes); 
        }
    }
    public void SetPlayerOn()
    {
        playerInputHandler.enabled = true;
        playerCollider.enabled = true;
        playerRigidBody.simulated = true;
        hearts.SetActive(true);
        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null)
        {
            inputManager.ChangeInputMap(PlayerInputManager.InputType.Player);
        }
        timerCutscene.enabled = true;
        this.enabled = false;
    }

}
