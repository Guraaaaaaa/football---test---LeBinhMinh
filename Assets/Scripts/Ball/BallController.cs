using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Kick Settings")]
    [SerializeField] float flightDuration = 1.5f;
    [SerializeField] float arcHeight = 3f;

    [Header("Effects")]
    [SerializeField] GameObject confettiPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip kickSound;
    [SerializeField] AudioClip goalSound;
    [SerializeField] AudioSource audioSource;

    // Sự kiện toàn cục để Camera hoặc các Manager khác có thể lắng nghe
    public static event Action<Transform> OnBallKicked;
    public static event Action<Vector3> OnBallReachedGoal;
     
    public bool IsKicked { get; private set; } = false;

    void Start()
    {
        if (confettiPrefab != null) confettiPrefab.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }
    public void KickToNearestGoal()
    {
        if (IsKicked) return;

        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
        if (goals.Length == 0)
        {
            Debug.LogWarning("Không tìm thấy object nào có tag 'Goal' trong scene!");
            return;
        }

        GameObject nearestGoal = null;
        float minDistance = float.MaxValue;

        foreach (var goal in goals)
        {
            float dist = Vector3.Distance(transform.position, goal.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestGoal = goal;
            }
        }

        if (nearestGoal != null)
        {
            Kick(nearestGoal.transform);
        }
    }

    public void Kick(Transform targetGoal)
    {
        if (IsKicked || targetGoal == null) return;
        
        IsKicked = true;

        if (audioSource != null && kickSound != null)
        {
            audioSource.PlayOneShot(kickSound);
        }

        OnBallKicked?.Invoke(transform);
        StartCoroutine(MoveBallToGoal(targetGoal.position));
    }

    IEnumerator MoveBallToGoal(Vector3 targetPos)
    {
        float t = 0;
        Vector3 startPos = transform.position;

        while (t < 1)
        {
            t += Time.deltaTime / flightDuration;
            
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            
            transform.position = currentPos;
            yield return null;
        }

        transform.position = targetPos;
        IsKicked = false;

        // bật hiệu ứng khi bóng vô lưới
        if (confettiPrefab != null)
        {
            confettiPrefab.SetActive(true);
            confettiPrefab.transform.position = targetPos;
            ParticleSystem ps = confettiPrefab.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        // Phát âm thanh lúc vào lưới
        if (audioSource != null && goalSound != null)
        {
            audioSource.PlayOneShot(goalSound);
        }
        
        // Gọi event báo hiệu đã tới goal 
        OnBallReachedGoal?.Invoke(targetPos);

        // Đợi 2 giây rồi tắt Partical System
        if (confettiPrefab != null)
        {
            yield return new WaitForSeconds(2f);
            confettiPrefab.SetActive(false);
        }
    }
}
