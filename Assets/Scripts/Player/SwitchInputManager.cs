using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class SwitchInputManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public Collider2D playerCollider;
    public Rigidbody2D playerRigidBody;
    public PlayerInputHandler playerInputHandler;
    public void SetPlayerOff()
    {
        playerInputHandler.enabled = false;
        playerCollider.enabled = false;
        playerRigidBody.simulated = false;


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
        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null)
        {
            inputManager.ChangeInputMap(PlayerInputManager.InputType.Player);
        }
    }
}
