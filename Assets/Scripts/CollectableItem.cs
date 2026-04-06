using DG.Tweening;
using UnityEngine;

public class CollectableItem : MonoBehaviour,IInteractable
{
    [SerializeField] private GameObject HandVersion;
    public void Interact()
    {
        PickUp();
    }
    private void PickUp()
    {
        if (HandVersion != null) 
        {
            HandVersion.SetActive(true);

            if(HandVersion.TryGetComponent(out FlashLight flash)) flash.enabled = true;
            if(HandVersion.TryGetComponent(out SwayController sway)) sway.enabled = true;

        }
        Destroy(gameObject);

        Debug.Log("Obje toplandý ve eldeki versiyon aktif edildi.");
        //if (this.TryGetComponent(out FlashLight flash))
        //{
        //    flash.enabled = true;
        //}
        //if (this.TryGetComponent(out SwayController sway))
        //{
        //    sway.enabled = true;
        //}
        //if (this.TryGetComponent(out Outline outline))
        //{
        //    outline.enabled = false;
        //}

        //transform.SetParent(hand);

        //transform.localScale = new Vector3(1f, 1f, 1f);

        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;

        //gameObject.layer = LayerMask.NameToLayer("Weapons");

        //Destroy(GetComponent<Outline>());
        //this.enabled = false;
    }

    public string GetInteractText()
    {
        return "'E' Flash Light";
    }

}
