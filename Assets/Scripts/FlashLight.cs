using UnityEngine;
using DG.Tweening;
public class FlashLight : MonoBehaviour
{
    //TELEFONUN BASINCA TELEFONUN EKRANINA TIKLAMA EFEKTÞ VEYA SES EFEKTÝ KONULACAK

    [Header("Light Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool canOpen = true;
    [SerializeField] private Light Light;
    [SerializeField] private float MaxIntensity = 5f;
    private float currentIntensity;

    [Header("Batery Settings")]
    [SerializeField] private float collectingBatteryHealth = 30f;//bunu toplanan bataryadan alcam toplama sistemini yapýnca güncellicem
    [SerializeField] private float batteryHealth = 30f;
    [SerializeField] private float batteryCount = 2;
    void Start()
    {
        currentIntensity = 0;
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (canOpen)
            {

                ToggleLight();
            }
        }
        if (isOpen)
        {
            batteryHealth -= Time.deltaTime;
        }
        if (batteryHealth <=0)
        {
            if(batteryCount >0)
            {
                batteryHealth = collectingBatteryHealth;
                batteryCount--;
                return;
            }

            if(isOpen)
                ToggleLight();

            batteryHealth = 0;
            canOpen = false;
        }
    }
    private void ToggleLight()
    {
        isOpen = !isOpen;
        currentIntensity = isOpen ? MaxIntensity : 0;
        Light.DOIntensity(currentIntensity, 0.1f);

    }
}
