using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BombMover : MonoBehaviour
{
    public Vector3 localOffset = new Vector3(5f, 0f, 0f);
    public float speed = 3f;
    public float stopDistance = 0.1f;
    public bool startAtA = true;

    private Vector3 pointA, pointB;
    private Vector3 target;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pointA = transform.position;
        pointB = pointA + localOffset;
        target = startAtA ? pointB : pointA;
    }

    void FixedUpdate()
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.magnitude <= stopDistance)
        {
            target = (target == pointA) ? pointB : pointA;
            dir = (target - transform.position);
            dir.y = 0f;
        }
        Vector3 move = dir.normalized * speed;
        rb.MovePosition(transform.position + move * Time.fixedDeltaTime);
    }
}