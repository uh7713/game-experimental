using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("攝影機設定")]
    [Tooltip("請把 player 底下的 Main Camera 拖進來")]
    public GameObject playerCamera; 
    
    [Tooltip("請把剛剛新建的 GodCamera 拖進來")]
    public GameObject godCamera;    

    void Start()
    {
        // 遊戲一開始，確保玩家視角是開啟的，上帝視角是關閉的
        if (playerCamera != null) playerCamera.SetActive(true);
        if (godCamera != null) godCamera.SetActive(false);
    }

    void Update()
    {
        // 當按下鍵盤的 H 鍵時
        if (Input.GetKeyDown(KeyCode.H))
        {
            // 檢查目前上帝視角是不是開著的
            bool isGodViewActive = godCamera.activeSelf;
            
            // 如果上帝視角開著，就關掉它，並打開玩家視角 (反之亦然)
            godCamera.SetActive(!isGodViewActive);
            playerCamera.SetActive(isGodViewActive);
        }
    }
}