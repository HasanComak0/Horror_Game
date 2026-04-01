using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] public float BobSpeed;//10
    [SerializeField] public float BobAmount;//0.05

    private float defaultYPos;
    private float timer;

    private void Start()
    {
        defaultYPos = transform.localPosition.y;
    }
    void Update()
    {
        float inputX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float inputY = Mathf.Abs(Input.GetAxis("Vertical"));

        // Eðer herhangi bir yönde hareket varsa (0.1'den büyükse) çalýþtýr
        if (inputX > 0.1f || inputY > 0.1f)
        {
            timer += Time.deltaTime * BobSpeed;

            float newYPos = defaultYPos + Mathf.Sin(timer) * BobAmount;//hareket ederken sürekli yukarý aþaðý hareket edicek.

            transform.localPosition = new Vector3(0, newYPos,0);
        }
        else
        {
            timer = 0;

            float smoothY = Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * 20f);
            transform.localPosition = new Vector3(0, smoothY, 0);
        }
    }
}
