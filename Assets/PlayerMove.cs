using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 取得輸入（A D 對應 Horizontal，W S 對應 Vertical）

        float moveX = Input.GetAxisRaw("Horizontal"); // A(-1) D(1)

        float moveZ = Input.GetAxisRaw("Vertical");   // S(-1) W(1)


        // 組成移動方向（X 左右、Z 前後）

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;


        // 移動（Time.deltaTime 讓移動不受 FPS 影響）

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);   
    }
}
