using UnityEngine;
using TMPro;

public class CameraHit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private float lookDistance = 50f;
    [SerializeField] private LayerMask lookableLayer;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance, lookableLayer))
        {
            Vector3 lookingObject = hit.collider.transform.position;
            if (hit.distance < 3f)
            {
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
        interactionText.gameObject.SetActive(false);
    }
}
