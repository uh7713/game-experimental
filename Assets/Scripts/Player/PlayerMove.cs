using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 1.8f;
    public float gravity = -9.81f;

    public Transform cameraTransform;

    private CharacterController controller;
    private float yVelocity; // 垂直速度

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // =====================
        // 1️⃣ 地面判斷
        // =====================
        if (controller.isGrounded)
        {
            if (yVelocity < 0)
                yVelocity = -2f; // 貼地，避免漂浮

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 跳躍初速度
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // =====================
        // 2️⃣ 重力
        // =====================
        yVelocity += gravity * Time.deltaTime;

        // =====================
        // 3️⃣ 移動輸入
        // =====================
        float moveX = Input.GetAxisRaw("Horizontal"); // A D
        float moveZ = Input.GetAxisRaw("Vertical");   // W S

        // Camera 方向（只取水平）
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camRight * moveX + camForward * moveZ;

        // =====================
        // 4️⃣ 整合 Y 軸
        // =====================
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
