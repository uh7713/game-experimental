using System.Collections;
using UnityEngine;
using UnityEngine.AI; // 必須引用 AI 命名空間

// 自動加入所需的元件
[RequireComponent(typeof(NavMeshObstacle))]
public class MovingWall : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("降下後的高度 (本地 Y 軸座標)")]
    // 請根據你的牆壁位置調整，我這裡預設為 -1.5f 保證它沉入地板
    [SerializeField] private float _downY = -1.5f; 
    
    [Tooltip("降下所需時間")]
    [SerializeField] private float _moveTime = 1.0f;

    private NavMeshObstacle _obstacle;
    private Vector3 _upLocalPos;
    private Vector3 _downLocalPos;
    private bool _isMoving = false; // 防止重複觸發

    // 公開屬性，讓 MazeGenerator 知道這面牆目前的狀態
    public bool IsDown { get; private set; } = false; 

    void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
        // 確保 Carve 被開啟
        if (_obstacle != null) _obstacle.carving = true;

        // 記錄起始位置（升起狀態）
        _upLocalPos = transform.localPosition;
        // 設定目標位置（降下狀態）
        _downLocalPos = new Vector3(_upLocalPos.x, _downY, _upLocalPos.z);
        
        // 遊戲一開始，牆壁預設是升起的狀態
        transform.localPosition = _upLocalPos;
    }

    // 公開方法：供午夜事件呼叫，將牆壁降下
    public void MoveDown()
    {
        if (_isMoving || IsDown) return; // 正在動或是已經在下，就不重複執行
        StopAllCoroutines(); // 停止可能的舊移動
        StartCoroutine(MoveRoutine(_downLocalPos, true));
    }

    // 公開方法：供黎明事件呼叫，將牆壁升起 (如果需要的話)
    public void MoveUp()
    {
        if (_isMoving || !IsDown) return;
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(_upLocalPos, false));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, bool goingDown)
    {
        _isMoving = true;
        
        // 1. 如果牆壁要下降，我們必須先關閉 NavMeshObstacle 的 Carve 功能
        //    這會讓系統在下一幀重新計算導航，讓怪物知道這條路變通了
        if (goingDown && _obstacle != null)
        {
            _obstacle.carving = false;
        }

        // 2. 平滑移動
        float elapsedTime = 0;
        Vector3 startPos = transform.localPosition;
        while (elapsedTime < _moveTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / _moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos; // 確保到達精確位置

        // 3. 移動完畢後的處理
        _isMoving = false;
        IsDown = goingDown;

        if (goingDown)
        {
            // 如果牆壁已經降下，完全停用 Obstacle，效能更好
            if (_obstacle != null) _obstacle.enabled = false;
        }
        else
        {
            // 如果牆壁要升起，先啟用 Obstacle，然後啟用 Carve 把網格「挖」出一個洞，阻擋怪物
            if (_obstacle != null)
            {
                _obstacle.enabled = true;
                _obstacle.carving = true;
            }
        }
    }
}