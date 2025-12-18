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
    private int _mazeWidth = 20; // 建議設大一點，例如 20

    [Tooltip("迷宮的複雜度：深度格子數")]
    [SerializeField] 
    private int _mazeDepth = 20; // 建議設大一點，例如 20

    [Tooltip("迷宮的物理大小：數字越大，走道越寬")]
    [SerializeField] 
    private float _cellSize = 2f; // 設為 1 是預設，設為 2 或 3 會變很寬

    private MazeCell[,] _mazeGrid;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                // 1. 計算物理位置 (乘上 cellSize)
                Vector3 position = new Vector3(x * _cellSize, 0, z * _cellSize);
                
                // 2. 生成格子
                MazeCell newCell = Instantiate(_mazeCellPrefab, position, Quaternion.identity);
                
                // 3. 調整格子本身的大小 (Scale)，這樣牆壁才會跟著變厚、變長
                newCell.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);

                // 4. 寫入座標索引 (防止報錯的關鍵)
                newCell.GridX = x;
                newCell.GridZ = z;
                
                // 5. 放入陣列
                _mazeGrid[x, z] = newCell;
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);
    }

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
        // === 關鍵修改：使用 GridX/Z 讀取座標，而不是 transform.position ===
        int x = currentCell.GridX;
        int z = currentCell.GridZ;

        // 檢查右邊
        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (cellToRight.IsVisited == false) yield return cellToRight;
        }

        // 檢查左邊
        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (cellToLeft.IsVisited == false) yield return cellToLeft;
        }

        // 檢查前邊
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (cellToFront.IsVisited == false) yield return cellToFront;
        }

        // 檢查後邊
        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (cellToBack.IsVisited == false) yield return cellToBack;
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        // 使用 GridX/Z 判斷相對位置，保證準確
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