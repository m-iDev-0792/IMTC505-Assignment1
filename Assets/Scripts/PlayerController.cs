using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;
    public float maxHorizontalSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer = ~0;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (GameManager.Instance != null)
            GameManager.Instance.player = transform;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        // 跳跃
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f; // 先清竖直速度，保证跳跃一致
            rb.linearVelocity = v;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // Debug.Log($"Player input h: {h}, v: {v}");
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;
        Vector3 desired = inputDir * moveSpeed;

        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        Vector3 delta = desired - horizontal;
        // Debug.Log($"Add force delta {delta}");
        rb.AddForce(delta, ForceMode.Acceleration);

        Vector3 limited = Vector3.ClampMagnitude(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z), maxHorizontalSpeed);
        rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
    }
    void FixedUpdate2()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");   

        Vector3 inputDir = new Vector3(h, 0f, v);
        rb.AddForce(inputDir * moveSpeed, ForceMode.VelocityChange);
    }
}