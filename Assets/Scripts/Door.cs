using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, IInteractable
{
    //KAPI FALSE DURUMUNDAYKEN COLLÝDERÝ KAPATCAM

    private bool isOpen = false;
    private bool isAnimating = false; // Animasyon sýrasýnda tekrar basýlmasýn diye
    [SerializeField] private Transform DoorHandle;

    public void Interact()
    {
        if (isAnimating) return;//kapý hareket ediyosa biþey yapmasýn

        //bu dalga yapýlacak iþlemleri sýraya almaya yarýyo ilk þunu yap sonra þunu sonra þunu diye.
        DG.Tweening.Sequence doorSeq = DOTween.Sequence();

        int ignoreLayer = LayerMask.NameToLayer("IgnorePlayer");
        int defaultLayer = LayerMask.NameToLayer("Interactable");

        isAnimating = true;
        if (!isOpen)
        {
            //gameObject.layer = ignoreLayer;
            float rotationValue = 0f;
            doorSeq.Append(
            DOTween.To(() => rotationValue, x => rotationValue = x, -30f, 0.3f)
                .OnUpdate(() =>
                {
                    // Her karede, deðiþen rotationValue'yu kolun açýsýna eþitliyoruz
                    DoorHandle.localRotation = Quaternion.Euler(-180f, rotationValue, 0f);
                }));

            doorSeq.Append(transform.DOLocalRotate(new Vector3(0, 0, -110), 1.5f));

            float newRotationValue = -30f;

            doorSeq.Append(
            DOTween.To(() => newRotationValue, x => newRotationValue = x, 0f, 0.3f)
                .OnUpdate(() =>
                {
                    // Her karede, deðiþen rotationValue'yu kolun açýsýna eþitliyoruz
                    DoorHandle.localRotation = Quaternion.Euler(-180f, newRotationValue, 0f);
                }));

            doorSeq.OnComplete(() =>
            {
                isOpen = true;
                isAnimating = false;
                //gameObject.layer = defaultLayer;
            });

        }
        else
        {
            //gameObject.layer = ignoreLayer;

            transform.DOLocalRotate(new Vector3(0, 0, 0), 1.5f).OnComplete(() =>
            {
                isOpen = false;
                isAnimating = false;
                //gameObject.layer = defaultLayer;
            });//iþ bittikten sonra isAnimating'i false yapýyo

        }
    }
    public string GetInteractText()
    {
        if (!isOpen)
            return "[E] Open";
        else
            return "[E] Close";
    }


}
