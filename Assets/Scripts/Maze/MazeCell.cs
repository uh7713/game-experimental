using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    [SerializeField]
    private GameObject _unvisitedBlock;

    public bool IsVisited { get; private set; }

    // === 關鍵新增：用來記錄座標索引 ===
    public int GridX; 
    public int GridZ;
    // =============================

    // === 新增：將 GameObject 開放給 Generator 讀取，方便 Generator 去找 MovingWall 腳本 ===
    public GameObject LeftWallObj => _leftWall;
    public GameObject RightWallObj => _rightWall;
    public GameObject FrontWallObj => _frontWall;
    public GameObject BackWallObj => _backWall;
    // ==============================================================================

    public void Visit()
    {
        IsVisited = true;
        if (_unvisitedBlock != null) _unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall() => _leftWall.SetActive(false);
    public void ClearRightWall() => _rightWall.SetActive(false);
    public void ClearFrontWall() => _frontWall.SetActive(false);
    public void ClearBackWall() => _backWall.SetActive(false);
}