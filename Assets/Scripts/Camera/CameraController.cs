using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] Transform playerTransform;

    [Header("Settings")]
    [SerializeField] Vector3 offset = new Vector3(0, 12f, -8f);
    
    [SerializeField] float followSpeed = 5f;

    [SerializeField] float focusbong = 2f;

    Transform currentTarget;

    void Start()
    {
        // Khởi tạo mục tiêu ban đầu là nhân vật
        currentTarget = playerTransform;

        // Tự động tìm nhân vật nếu chưa gán
        if (playerTransform == null)
        {
            Player p = FindObjectOfType<Player>();
            if (p != null) 
            {
                playerTransform = p.transform;
                currentTarget = playerTransform;
            }
        }

        BallController.OnBallKicked += HandleBallKicked;
    }

    void OnDestroy()
    {
        BallController.OnBallKicked -= HandleBallKicked;
    }

    void HandleBallKicked(Transform ballTransform)
    {
        if (gameObject.activeInHierarchy)
        {
            // Ngay khi bóng bị đá, gọi Coroutine để focus vào bóng
            StartCoroutine(FocusOnBallRoutine(ballTransform));
        }
    }

    IEnumerator FocusOnBallRoutine(Transform ballTransform)
    {
        currentTarget = ballTransform;

        yield return new WaitForSeconds(focusbong);

        currentTarget = playerTransform;
    }

    // Dùng LateUpdate cho camera để tránh giật lag khi follow object di chuyển ở Update
    void LateUpdate()
    {
        if (currentTarget == null) return;

        Vector3 targetPosition = currentTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.LookAt(currentTarget.position);
    }
}
