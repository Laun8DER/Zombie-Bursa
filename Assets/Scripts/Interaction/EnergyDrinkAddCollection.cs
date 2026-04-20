using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.UI;
public class EnergyDrinkAddCollection : MonoBehaviour
{
    [Header("Collect Animation")]
    public GameObject Monster_UI;
    public Animator Monster_Item_Animator;
    public Animator Monster_BG_Animator;
    public Animator Monster_FADE_Animator;
    [Header("Pickup")]
    public bool MonsterPicked = false;
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

        //COLLECT ANIMATION
        if (MonsterPicked == false)
        {
            MonsterPicked = true;
            StartCoroutine(PlayCollectAnimation());
            return;
        }
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }

    }

    IEnumerator PlayCollectAnimation()
    {
        Monster_UI.SetActive(true);
            Monster_Item_Animator.Play("Monster_Item");
            Monster_BG_Animator.Play("Monster_BG");
            Monster_FADE_Animator.Play("Monster_Fading");

        yield return new WaitForSeconds(Monster_FADE_Animator.GetCurrentAnimatorStateInfo(0).length);
        //
        Monster_UI.SetActive(false);
        Destroy(gameObject);


    }
}
