using UnityEngine;
public class EnergyDrinkPickup : MonoBehaviour
{

    [Header("Pickup")]
    public string playerTag = "Player";
    public float staminaRestoreAmount = 25f;
    public bool destroyOnPickup = true;

    private bool wasPickedUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wasPickedUp || other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        BurpAttack burpAttack = other.GetComponent<BurpAttack>();
        if (burpAttack == null)
        {
            burpAttack = other.GetComponentInParent<BurpAttack>();
        }

        if (burpAttack == null)
        {
            return;
        }

        burpAttack.RestoreStamina(staminaRestoreAmount);
        wasPickedUp = true;

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }

    }
}
