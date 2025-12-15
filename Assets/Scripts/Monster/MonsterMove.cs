using UnityEngine;
using UnityEngine.AI;

public class WeepingAngel : MonoBehaviour
{
    public Transform player;       // 玩家 (攝影機)
    public Transform playerCamera; // 或是直接拖入 Main Camera
    
    private NavMeshAgent agent;
    private Renderer myRenderer;   // 用來取得物體邊界

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myRenderer = GetComponent<Renderer>();
        
        if (playerCamera == null) playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (player == null) return;

        // 核心邏輯：如果被看見，就停下；否則，追擊
        if (IsSeenByCamera())
        {
            agent.isStopped = true; // 停止導航
            agent.velocity = Vector3.zero; // 立即煞車，防止慣性滑動
        }
        else
        {
            agent.isStopped = false; // 恢復導航
            agent.SetDestination(player.position);
        }
    }

    // 判斷是否被攝影機看見
    bool IsSeenByCamera()
    {
        Camera cam = playerCamera.GetComponent<Camera>();

        // 1. 【畫面檢查】將怪物的世界座標轉為「視口座標 (Viewport)」
        // 視口座標：(0,0)是左下角，(1,1)是右上角。Z>0 代表在攝影機前方
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

        bool onScreen = viewPos.x >= 0 && viewPos.x <= 1 && 
                        viewPos.y >= 0 && viewPos.y <= 1 && 
                        viewPos.z > 0;

        if (!onScreen) return false; // 如果根本不在畫面裡，直接回傳沒看見

        // 2. 【牆壁遮擋檢查】發射一條射線 (Raycast)
        // 從攝影機射向怪物
        RaycastHit hit;
        Vector3 directionToEnemy = transform.position - playerCamera.position;
        
        if (Physics.Raycast(playerCamera.position, directionToEnemy, out hit))
        {
            // 如果射線打到的第一個東西是「我自己」，代表中間沒有牆壁
            if (hit.transform == transform)
            {
                return true; // 真的被看見了！
            }
        }

        return false; // 中間有牆壁擋住
    }
}