using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("迷宮設定")]
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [Tooltip("迷宮的複雜度：寬度格子數")]
    [SerializeField] 
    private int _mazeWidth = 20;

    [Tooltip("迷宮的複雜度：深度格子數")]
    [SerializeField] 
    private int _mazeDepth = 20;

    [Tooltip("迷宮的物理大小：數字越大，走道越寬")]
    [SerializeField] 
    private float _cellSize = 2f;

    [Header("牆壁移動機關設定")]
    [Tooltip("每次午夜時會同時變動的牆壁數量")]
    [SerializeField] 
    private int _numberOfWallsToMove = 5;

    private MazeCell[,] _mazeGrid;
    
    // === 牆壁機關專用清單 ===
    // 記錄目前「升起」且可以被當作機關降下的實體牆壁
    private List<MovingWall> _solidWallsPool = new List<MovingWall>();
    // 記錄目前「已經降下」的牆壁，下次午夜要把它們升起來
    private List<MovingWall> _loweredWalls = new List<MovingWall>();

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        // 1. 生成所有基礎格子
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector3 position = new Vector3(x * _cellSize, 0, z * _cellSize);
                MazeCell newCell = Instantiate(_mazeCellPrefab, position, Quaternion.identity);
                newCell.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                newCell.GridX = x;
                newCell.GridZ = z;
                _mazeGrid[x, z] = newCell;
            }
        }

        // 2. 執行演算法打通迷宮路線
        GenerateMaze(null, _mazeGrid[0, 0]);

        // 3. 收集所有留下來的「實體牆壁」，準備作為夜晚機關
        CollectSolidWalls();
    }

    void Update()
    {
        // 測試用：按下 M 鍵觸發午夜機關
        if (Input.GetKeyDown(KeyCode.M))
        {
            TriggerMidnightEvent();
        }
    }

    // ==========================================
    //          牆壁移動機關核心邏輯
    // ==========================================

    private void CollectSolidWalls()
    {
        _solidWallsPool.Clear();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                MazeCell cell = _mazeGrid[x, z];
                
                // 檢查四面牆，如果是實體牆就加進清單
                CheckAndAddWall(cell.LeftWallObj);
                CheckAndAddWall(cell.RightWallObj);
                CheckAndAddWall(cell.FrontWallObj);
                CheckAndAddWall(cell.BackWallObj);
            }
        }
        Debug.Log($"迷宮生成完畢！共收集到 {_solidWallsPool.Count} 面牆壁作為機關。");
    }

    private void CheckAndAddWall(GameObject wallObj)
    {
        // 如果牆壁物件存在且處於開啟狀態(沒被演算法隱藏)
        if (wallObj != null && wallObj.activeSelf)
        {
            MovingWall mw = wallObj.GetComponent<MovingWall>();
            if (mw != null)
            {
                _solidWallsPool.Add(mw);
            }
        }
    }

    public void TriggerMidnightEvent()
    {
        if (_solidWallsPool == null || _solidWallsPool.Count == 0) return;

        // 1. 先把上次降下的牆壁備份起來，準備升起
        List<MovingWall> wallsToRise = new List<MovingWall>(_loweredWalls);
        _loweredWalls.Clear(); 

        // 2. 從目前的「實體牆壁池」中，隨機挑選新的一批牆壁準備降下
        int wallsToMoveCount = Mathf.Min(_numberOfWallsToMove, _solidWallsPool.Count);
        List<MovingWall> selectedWallsToDrop = _solidWallsPool
            .OrderBy(_ => Random.value) // 隨機打亂
            .Take(wallsToMoveCount)     // 取前幾面
            .ToList();

        // 3. 執行降下動作 (新的路線開啟)
        foreach (MovingWall wall in selectedWallsToDrop)
        {
            wall.MoveDown();
            _solidWallsPool.Remove(wall); // 從可降下清單移除
            _loweredWalls.Add(wall);      // 加入已降下清單
        }

        // 4. 執行升起動作 (舊的路線封死)
        foreach (MovingWall wall in wallsToRise)
        {
            wall.MoveUp();
            _solidWallsPool.Add(wall); // 牆壁升起來了，重新加回可用清單
        }

        Debug.Log($"午夜轟鳴！升起了 {wallsToRise.Count} 面舊牆，降下了 {selectedWallsToDrop.Count} 面新牆！");
    }


    // ==========================================
    //          原本的迷宮生成演算法
    // ==========================================

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = currentCell.GridX;
        int z = currentCell.GridZ;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (cellToRight.IsVisited == false) yield return cellToRight;
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (cellToLeft.IsVisited == false) yield return cellToLeft;
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (cellToFront.IsVisited == false) yield return cellToFront;
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (cellToBack.IsVisited == false) yield return cellToBack;
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.GridX < currentCell.GridX)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.GridX > currentCell.GridX)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.GridZ < currentCell.GridZ)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.GridZ > currentCell.GridZ)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
}