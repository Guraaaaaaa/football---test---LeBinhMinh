using System;
using UnityEngine;

public class BallDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Bán kính vùng phát hiện bóng xung quanh nhân vật")]
    [SerializeField] float detectionRadius = 2.5f;
    
    [Tooltip("Layer dành riêng cho bóng để tối ưu hóa việc quét (ví dụ: tạo layer 'Ball')")]
    [SerializeField] LayerMask ballLayer;

    public static event Action<bool, BallController> OnNearBallChanged;

    bool isNearBall = false;
    BallController closestBall = null;

    void Update()
    {
        DetectBall();
    }

    void DetectBall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, ballLayer);
        bool currentlyNear = hits.Length > 0;

        if (currentlyNear)
        {
            // Lấy bóng khi quét mảng sẽ lấy bóng đầu tiên khi quét
            BallController foundBall = hits[0].GetComponentInParent<BallController>();
            
            // phát hiện trái bóng lucvs vào vùng
            if (!isNearBall || closestBall != foundBall)
            {
                isNearBall = true;
                closestBall = foundBall;
                OnNearBallChanged?.Invoke(true, closestBall); // Báo hiệu hiện nút Kick
            }
        }
        else
        {
            // Nếu vừa mới rời khoir vùng bóng
            if (isNearBall)
            {
                isNearBall = false;
                closestBall = null;
                OnNearBallChanged?.Invoke(false, null); // Báo hiệu ẩn nút Kick
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
