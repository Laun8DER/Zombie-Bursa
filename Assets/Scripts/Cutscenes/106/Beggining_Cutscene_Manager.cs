using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;   

public class Beggining_Cutscene_Manager : MonoBehaviour
{
    public GameObject doorInteract;
    public PlayerInput playerInput; 
    public Collider2D playerCollider;
    public Rigidbody2D playerRigidBody;
    public GameObject hearts;
    public PlayableDirector WakeUpCutscene;
    public PlayableDirector SleepCutscene;
    public GameObject wakeUpButton;
    public PlayerInputHandler playerInputHandler;
    public GameObject BurpBar;
    public Image FadeScreen;


    private void Start()
    {
        SetDoorInteractOff();
        SetPlayerOff();
        SetBurpBarOff();
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
    public void SetDoorInteractOn()
    {
        doorInteract.SetActive(true);
    }
    public void SetDoorInteractOff()
    {
        doorInteract.SetActive(false);
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
        this.enabled = false;
    }
    public void SetBurpBarOff()
    {
        BurpBar.SetActive(false);
    }
    public void SetBurpBarOn()
    {
        BurpBar.SetActive(true);
    }
    public void setFadeScreenOff()
    {
        FadeScreen.gameObject.SetActive(false);
    }
}
