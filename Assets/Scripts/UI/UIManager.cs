using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] Button kickButton;
    [SerializeField] Button autoKickButton;
    [SerializeField] Button resetButton;

    [Header("Game UI")]
    [SerializeField] Text scoreText;
    [SerializeField] GameObject winPanel;
    
    [SerializeField] Button winResetButton;

    [Header("References")]
    [SerializeField] Transform playerTransform;

    BallController currentNearbyBall;
    
    int currentScore = 0;
    const int WIN_SCORE = 5;

    void Start()
    {
        if (kickButton != null)
        {
            kickButton.gameObject.SetActive(false);
            kickButton.onClick.AddListener(OnKickClicked);
        }

        if (autoKickButton != null) autoKickButton.onClick.AddListener(OnAutoKickClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        if (winResetButton != null) winResetButton.onClick.AddListener(OnResetClicked);

        // Đăng ký lắng nghe sự kiện từ BallDetector và BallController
        BallDetector.OnNearBallChanged += HandleNearBallChanged;
        BallController.OnBallReachedGoal += HandleBallReachedGoal;

        // Reset UI
        if (winPanel != null) winPanel.SetActive(false);
        UpdateScoreUI();

    if (playerTransform == null)
        {
            Player p = FindObjectOfType<Player>();
            if (p != null) playerTransform = p.transform;
        }
    }

    void OnDestroy()
    {
        BallDetector.OnNearBallChanged -= HandleNearBallChanged;
        BallController.OnBallReachedGoal -= HandleBallReachedGoal;
    }

    void HandleNearBallChanged(bool isNear, BallController ball)
    {
        currentNearbyBall = ball;
        
        // Hiện nút Kick nếu gần bóng VÀ quả bóng đó chưa bị sút
        if (kickButton != null)
        {
            bool showButton = isNear && ball != null && !ball.IsKicked;
            kickButton.gameObject.SetActive(showButton);
        }
    }

    void HandleBallReachedGoal(Vector3 goalPos)
    {
        currentScore++;
        UpdateScoreUI();
        if (currentScore >= WIN_SCORE)
        {
            if (winPanel != null) winPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore + " / " + WIN_SCORE;
        }
    }

    public void OnKickClicked()
    {
        if (currentNearbyBall != null && !currentNearbyBall.IsKicked)
        {
            currentNearbyBall.KickToNearestGoal();
            if (kickButton != null) kickButton.gameObject.SetActive(false);
            currentNearbyBall = null;
        }
    }

    public void OnAutoKickClicked()
    {
        if (playerTransform == null) return;

        // Tìm toàn bộ bóng trên sân
        BallController[] allBalls = FindObjectsOfType<BallController>();
        
        BallController furthestBall = null;
        float maxDistance = -1f;

        // Quét tìm bóng xa nhất chưa được sút
        foreach (BallController ball in allBalls)
        {
            if (ball.IsKicked) continue; 

            float dist = Vector3.Distance(playerTransform.position, ball.transform.position);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestBall = ball;
            }
        }

        if (furthestBall != null)
        {
            furthestBall.KickToNearestGoal();
        }
        else
        {
            Debug.Log("Không còn quả bóng nào trên sân để Auto Kick!");
        }
    }

    public void OnResetClicked()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
