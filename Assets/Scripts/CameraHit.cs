using UnityEngine;
using TMPro;

public class CameraHit : MonoBehaviour
{
    //YERDEN TOPLAMA SÝSTEMÝNÝ YAPARKEN TOPLANILAN OBJENÝN LAYER'INI Weapons YAPMAM LAZIM

    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private float lookDistance = 50f;
    [SerializeField] private LayerMask lookableLayer;
    private Outline currentOutline;
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, lookDistance, lookableLayer))
        {

            //Vector3 lookingObject = hit.collider.transform.position;
            if (hit.distance < 3f)
            {
                // Raycast'in çarptýðý objede VEYA alt objelerinde ÝLK bulduðun Outline'ý getir
                Outline outline = hit.collider.GetComponentInChildren<Outline>();


                if (outline != null)
                {
                    if (currentOutline != outline)//SADECE YENÝ BÝR ÞEYE BAKTIÐIMIZDA ÇALIÞIYO
                    {
                        if (currentOutline != null) 
                            currentOutline.enabled = false; // Eskisini söndür

                        currentOutline = outline;
                        currentOutline.enabled = true; // Yenisini tak
                        Debug.Log("Sadece bir kere yazar: Outline Bulundu");
                    }
                }

                if (hit.collider.TryGetComponent(out IInteractable Iinteractable))
                {
                    interactionText.text = Iinteractable.GetInteractText();
                    interactionText.gameObject.SetActive(true);


                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("E basýldý");
                        Iinteractable.Interact();

                    }
                    return;
                }
            }
        }
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null; 
        }
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}
