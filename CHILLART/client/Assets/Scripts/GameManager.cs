using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public string serverUrl = "http://localhost:3000";
    
    [Header("UI References")]
    public GameObject gamePanel;
    public Text gameTitleText;
    public Text scoreText;
    public Button backToLobbyButton;
    public Button submitScoreButton;
    
    [Header("Game State")]
    public string currentGameType = "";
    public int currentScore = 0;
    
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Setup UI buttons
        if (backToLobbyButton != null)
            backToLobbyButton.onClick.AddListener(BackToLobby);
            
        if (submitScoreButton != null)
            submitScoreButton.onClick.AddListener(SubmitScore);
            
        // Hide game panel initially
        if (gamePanel != null)
            gamePanel.SetActive(false);
    }
    
    public void StartGame(string gameType)
    {
        currentGameType = gameType;
        currentScore = 0;
        
        // Show game panel
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
            gameTitleText.text = $"{gameType} Game";
        }
        
        // Start the actual game logic
        StartCoroutine(SimulateGame());
    }
    
    private IEnumerator SimulateGame()
    {
        // Simulate game progress
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.5f);
            currentScore += Random.Range(10, 50);
            UpdateScoreDisplay();
        }
        
        // Game finished
        Debug.Log($"Game finished! Final score: {currentScore}");
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }
    
    public void BackToLobby()
    {
        if (gamePanel != null)
            gamePanel.SetActive(false);
            
        // Reset game state
        currentGameType = "";
        currentScore = 0;
    }
    
    public void SubmitScore()
    {
        if (string.IsNullOrEmpty(currentGameType) || currentScore <= 0)
        {
            Debug.LogWarning("Cannot submit score: invalid game state");
            return;
        }
        
        StartCoroutine(SubmitScoreToServer());
    }
    
    private IEnumerator SubmitScoreToServer()
    {
        // Get Firebase UID
        string uid = FirebaseManager.Instance?.GetCurrentUserUID();
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("No Firebase UID available");
            yield break;
        }
        
        // Prepare score data
        var scoreData = new
        {
            uid = uid,
            points = currentScore,
            gameType = currentGameType
        };
        
        string jsonData = JsonUtility.ToJson(scoreData);
        
        // Send POST request
        using (var request = new UnityEngine.Networking.UnityWebRequest($"{serverUrl}/score", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted successfully!");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to submit score: " + request.error);
            }
        }
    }
    
    public void LoadLeaderboard()
    {
        StartCoroutine(FetchLeaderboard());
    }
    
    private IEnumerator FetchLeaderboard()
    {
        using (var request = UnityEngine.Networking.UnityWebRequest.Get($"{serverUrl}/leaderboard"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log("Leaderboard data: " + request.downloadHandler.text);
                // Parse and display leaderboard data
            }
            else
            {
                Debug.LogError("Failed to fetch leaderboard: " + request.error);
            }
        }
    }
}
