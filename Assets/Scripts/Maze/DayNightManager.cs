using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [Header("時間設定")]
    [Tooltip("現實中的幾秒鐘，等於遊戲裡的一天")]
    [SerializeField] private float _dayDuration = 60f; // 預設 60 秒為一天，測試時可以設短一點

    [Tooltip("目前遊戲時間 (0.0 ~ 24.0)")]
    [Range(0f, 24f)]
    public float currentTime = 12f; // 預設從中午 12 點開始

    [Header("環境與事件綁定")]
    [Tooltip("請把場景中的 Directional Light 拖進來")]
    [SerializeField] private Light _sunLight;

    [Tooltip("請把掛有 MazeGenerator 的物件拖進來")]
    [SerializeField] private MazeGenerator _mazeGenerator;

    // 用來確保每天晚上的機關只會觸發一次
    private bool _hasTriggeredMidnight = false;

    void Update()
    {
        // 1. 計算時間流逝
        // 一天有 24 小時，計算出每秒要增加的遊戲時間
        float timeMultiplier = 24f / _dayDuration;
        currentTime += Time.deltaTime * timeMultiplier;

        // 2. 處理跨日邏輯 (超過 24 點就回到 0 點)
        if (currentTime >= 24f)
        {
            currentTime %= 24f; 
            _hasTriggeredMidnight = false; // 迎接新的一天，重置午夜觸發器
        }

        // 3. 旋轉太陽，改變場景光影
        UpdateSunRotation();

        // 4. 午夜事件觸發 (設定在 0 點剛過的時候觸發)
        // 確保時間大於 0，且尚未觸發過
        if (currentTime >= 0f && currentTime < 1f && !_hasTriggeredMidnight)
        {
            TriggerMidnight();
            _hasTriggeredMidnight = true;
        }
    }

    private void UpdateSunRotation()
    {
        if (_sunLight == null) return;

        // 讓太陽的旋轉角度跟著時間變化
        // 早上 6 點 = 0 度 (剛從地平線升起)
        // 中午 12 點 = 90 度 (正上方直射)
        // 傍晚 18 點 = 180 度 (落下地平線)
        // 午夜 0 點 = 270 度 (在正下方)
        
        float sunAngle = (currentTime - 6f) / 24f * 360f;
        
        // 改變 X 軸的旋轉來模擬太陽升降
        _sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);
    }

    private void TriggerMidnight()
    {
        if (_mazeGenerator != null)
        {
            _mazeGenerator.TriggerMidnightEvent();
            Debug.Log("🕛 時間到！系統自動觸發午夜迷宮變形！");
        }
    }
}