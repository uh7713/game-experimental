using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 8f;      // 跳躍力量
    private Rigidbody rb;
    private bool isGrounded = true;   // 是否在地面上

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 只要接觸到地板就恢復可跳狀態
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
