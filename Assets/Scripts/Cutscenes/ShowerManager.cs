using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class ShowerManager : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;

    public GameObject zombieObject;
    public GameObject interactableShowerObject;
    public GameObject interactButton;

    public void ChangeSprite()
    {
        spriteRenderer.sprite = newSprite;
    }
    public void DestroyShowerEvents()
    {
        Destroy(zombieObject);
        Destroy(interactableShowerObject);
        Destroy(interactButton);
        Destroy(gameObject);
    }
}
