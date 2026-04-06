using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerDoorInteractor : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    private readonly List<DoorTeleport> nearbyDoors = new List<DoorTeleport>();

    private void Awake()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
    }
    private void OnEnable()
    {
        PlayerInputManager.Instance.actions.Player.Interact.performed += TryInteract;
    }
    private void OnDisable()
    {
        PlayerInputManager.Instance.actions.Player.Interact.performed -= TryInteract;
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        DoorTeleport nearestDoor = null;
        float nearestDistance = float.MaxValue;

        for (int i = nearbyDoors.Count - 1; i >= 0; i--)
        {
            DoorTeleport door = nearbyDoors[i];
            if (door == null)
            {
                nearbyDoors.RemoveAt(i);
                continue;
            }

            if (!door.CanInteract(transform))
            {
                continue;
            }

            float distance = ((Vector2)transform.position - (Vector2)door.transform.position).sqrMagnitude;
            if (distance >= nearestDistance)
            {
                continue;
            }

            nearestDistance = distance;
            nearestDoor = door;
        }

        if (nearestDoor != null)
        {
            nearestDoor.Interact(transform, playerRigidbody);
            nearbyDoors.Clear();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DoorTeleport door = other.GetComponent<DoorTeleport>();
        if (door == null)
        {
            door = other.GetComponentInParent<DoorTeleport>();
        }

        if (door == null || nearbyDoors.Contains(door))
        {
            return;
        }

        nearbyDoors.Add(door);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        DoorTeleport door = other.GetComponent<DoorTeleport>();
        if (door == null)
        {
            door = other.GetComponentInParent<DoorTeleport>();
        }

        if (door == null)
        {
            return;
        }

        nearbyDoors.Remove(door);
    }
}
