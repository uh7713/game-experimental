using UnityEngine;

public class FacePlayerYAxis : MonoBehaviour
{
    public Transform targetCamera;

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 1. 取得攝影機的位置
        Vector3 targetPosition = targetCamera.position;

        // 2. 【關鍵】強制將目標高度 (y) 設為物體自身的高度
        targetPosition.y = transform.position.y;

        // 3. 讓物體看著這個「同高度」的目標點
        transform.LookAt(targetPosition);
    }
}