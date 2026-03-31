using UnityEngine;

public class PlayerMoveAndLook : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float extraGravity;
    [SerializeField] LayerMask ground;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float groundCheckRadius;
    Vector3 direction;
    Rigidbody rb;

    [Header("Mouse Look")]
    [SerializeField] private float sensivity;
    [SerializeField] Camera fpsCam;
    float mouseX, mouseY;
    float rotX, rotY;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        GetMouseAxis();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }
        if (rb.linearVelocity.y < 0f) 
        {
            rb.AddForce(Vector3.down*extraGravity,ForceMode.Acceleration);
        }
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    private void LateUpdate()
    {
        fpsCam.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }
    private void GetMouseAxis()
    {
        mouseX = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 100f);

        rotY += mouseX;
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
    }
    private void PlayerMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        direction = new Vector3(hor, 0, ver);

        rb.MovePosition(transform.position + transform.TransformDirection(direction * Time.fixedDeltaTime * moveSpeed));
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }
    private bool isGrounded()
    {
        if (Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, ground))
            return true;
        else
            return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
    }
}
