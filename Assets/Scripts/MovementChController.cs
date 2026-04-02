using NUnit.Framework;
using System.Collections.Generic;//AudioClip için eklendi
using System.Threading;
using UnityEngine;

public class MovementChController : MonoBehaviour
{
    //Zýplamaya koþul eklenecek belli yerlerde zýplayabileceðiz.
    //kameraya collider gibi biþey eklenecek objelerin içine girmemesi için
    [Header("Footstep Volume Settings")]
    [UnityEngine.Range(0f, 1.0f)]
    [SerializeField] private float masterVolume = 1.0f;

    [Header("Mouse Settings")]
    [SerializeField] private float sensivity;
    [SerializeField] Camera fpsCam;
    float mouseX, mouseY;
    float rotX, rotY;

    [Header("Movement Settings")]
    [SerializeField] private float increaseTime = 2f;//hýzý düþürme saniyesi yani bu kadar sürede hýz deðiþicek
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private bool isRunning;
    [SerializeField] private float crouchingSpeed = 2.5f;//karakter eðilince bu hýzda yürücek
    [SerializeField] private float currentSpeed;//mevcut hýz
    CharacterController characterController;
    public HeadBobbing bobbing;

    [Header("Jumping Settings")]
    [SerializeField] private float jumpHeight;

    [Header("Crouching Settings")]
    [SerializeField] private bool isCrouching;//Eðer Þuan eðiliyosam ve yürüyosam ses çýkartýcam
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchHeight = 1.3f; // eðilince olacak yükseklik
    [SerializeField] private float crouchSpeed = 5f;  // Geçiþ hýzý (1f çok yavaþtýr)
    [SerializeField] private bool stillCrouching;//hala çömeliyomuyum diye ekledim bunu eðilince kafamýzýn üstünde biþey varsa ayaða kalkamasýn diye koydum
    [SerializeField] private float RayDistance = 0.8f;//Kürenin yarýçapý


    [Header("Crouching Camera Settings")]
    [SerializeField] private GameObject CameraHolder;//eðilince kamera da eðiliyo ya o ayarlandý
    [SerializeField] private float normalPos = 0.8f;
    [SerializeField] private float crouchPos = 0.2f;

    [Header("Footstep Settings")]
    [SerializeField] private List<AudioClip> walkSteps = new List<AudioClip>();
    //[SerializeField] private List<AudioClip> metalWalkSteps = new List<AudioClip>(); //YÜZEY KONTROLÜ YAPINCA EKLENECEK.
    private AudioSource audioSource;
    private float footStepTimer;


    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f;
    Vector3 velocity;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        //SPRÝNT
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            //Mathf.Lerp(nereden, nereye, ne kadar_hýzlý)
            //Mantýðý þu: "A noktasýndan B noktasýna, þu kadar hýzla (t) git."
            isRunning = true;
            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, increaseTime * Time.deltaTime);
            bobbing.BobSpeed = 15;
        }
        else
        {
            isRunning = false;
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, increaseTime * Time.deltaTime);
            bobbing.BobSpeed = 10;
        }

        SetCrouching();
        GetMouseAxis();
        Movement();
        Jumping();
    }
    private void Movement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");


        //burda direk hareket ediyo yukardaki sadece koþmak için hýzý arttýrýcam normal olarak...
        Vector3 direction = (transform.right * hor + transform.forward * ver).normalized;
        characterController.Move(direction * currentSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            doFootStep();
        }

        //GRAVÝTY
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    private void doFootStep()
    {
        if (audioSource == null || walkSteps.Count == 0) return;

        footStepTimer -= Time.deltaTime;
        if (footStepTimer <= 0)
        {

            if (isCrouching)
            {
                audioSource.PlayOneShot(walkSteps[Random.Range(0, walkSteps.Count)]);
                audioSource.volume = 0.25f * masterVolume;
                footStepTimer = 0.9f;
            }
            else if (isRunning)
            {
                audioSource.PlayOneShot(walkSteps[Random.Range(0, walkSteps.Count)]);
                audioSource.volume = 1f * masterVolume;
                footStepTimer = 0.35f;
            }
            else
            {
                audioSource.PlayOneShot(walkSteps[Random.Range(0, walkSteps.Count)]);
                audioSource.volume = 0.5f * masterVolume;
                footStepTimer = 0.65f;
            }
        }

    }

    private void GetMouseAxis()
    {
        mouseX = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        rotY += mouseX;

        fpsCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
    }
    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            //Sqrt karekök alýyo ya bunu kullanmamýzýn sebebi aslýnda ne kadar yukarý zýplamak istediðimiz...
            //JumpHeight aslýnda kaç metre zýplayacaðýmýz.
            //burada yazýlan -2 deðeri de karekök içinin negatif sonuç vermemesi için verirse hata verir çünkü 
            //gravity de mevcut yerçekimimiz aslýnda
            //-2 olmasýnýn sebebi de matematiksel olarak formülün istediðiymiþ
        }
    }
    private void SetCrouching()//Eðilme
    {
        //GELÝÞTÝRÝLMEK ÝSTENÝRSE RAYCAST YERÝNE BAÞKA BÝRÞEY KULLANMAM LAZIM ÇOK ÝNCE OLDUUÐU ÝÇÝN BAZEN YUKARI DEÐEMÝYO

        stillCrouching = Physics.Raycast(CameraHolder.transform.position + (Vector3.up * 0.15f), Vector3.up, RayDistance, ~(1 << LayerMask.NameToLayer("Player")));
        Debug.DrawRay(CameraHolder.transform.position + (Vector3.up * 0.15f), Vector3.up * RayDistance, Color.red);


        // 1. Hedef boyu belirle
        float targetHeight;
        if (Input.GetKey(KeyCode.LeftControl) || stillCrouching)
        {
            targetHeight = crouchHeight;
            currentSpeed = Mathf.Lerp(currentSpeed, crouchingSpeed, increaseTime);//eðilince karakterin hýzýný düþürdüm
            bobbing.BobSpeed = 7;
            bobbing.BobAmount = 0.07f;
            CameraHolder.transform.localPosition = new Vector3(0, Mathf.Lerp(CameraHolder.transform.localPosition.y, crouchPos, crouchSpeed * Time.deltaTime), 0);       
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W))
            {
                isCrouching = true;
            }

        }
        else
        {
            targetHeight = normalHeight;
            isCrouching = false;
            CameraHolder.transform.localPosition = new Vector3(0, Mathf.Lerp(CameraHolder.transform.localPosition.y, normalPos, crouchSpeed * Time.deltaTime), 0);
        }


        // 2. Mevcut boyu (characterController.height) hedefe doðru yumuþat (Lerp)
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchSpeed * Time.deltaTime);

        // 3. ÇOK ÖNEMLÝ: Karakterin ayaklarýnýn yerden kesilmemesi için merkezi (Center) de güncellemelisin
        // Boy yarýya indiðinde merkez de aþaðý kaymalý.
        float targetCenterY = targetHeight / 2f;

        // CharacterController'ýn pivot noktasý genelde merkezdedir, 
        // Ayaklarýn yere basmasý için height deðiþtikçe center.y'yi de ayarlýyoruz.
        Vector3 newCenter = characterController.center;
        newCenter.y = characterController.height / 2f - (normalHeight / 2f);
        // Üstteki formül karýþýk gelirse þunu dene:
        // newCenter.y = Input.GetKey(KeyCode.LeftControl) ? -0.5f : 0f; 

        characterController.center = newCenter;
    }
}
