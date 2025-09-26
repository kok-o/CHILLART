using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;
    public GameObject gamePanel;
    public GameObject leaderboardPanel;
    public GameObject settingsPanel;
    
    [Header("Lobby UI Elements")]
    public Text playerNameText;
    public Text playerScoreText;
    public Button leaderboardButton;
    public Button settingsButton;
    public Button signOutButton;
    
    [Header("Game Panel Elements")]
    public Text gameTitleText;
    public Text gameScoreText;
    public Button backToLobbyButton;
    public Button submitScoreButton;
    public Slider progressSlider;
    
    [Header("Leaderboard Elements")]
    public Transform leaderboardContent;
    public GameObject leaderboardItemPrefab;
    public Button refreshLeaderboardButton;
    public Button closeLeaderboardButton;
    
    [Header("Settings Elements")]
    public Slider volumeSlider;
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Button closeSettingsButton;
    
    private GameManager gameManager;
    private FirebaseManager firebaseManager;
    
    void Start()
    {
        // Get managers
        gameManager = GameManager.Instance;
        firebaseManager = FirebaseManager.Instance;
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Initialize UI
        InitializeUI();
    }
    
    void Update()
    {
        // Update player info
        UpdatePlayerInfo();
        
        // Update game score if in game
        if (gamePanel != null && gamePanel.activeInHierarchy)
        {
            UpdateGameScore();
        }
    }
    
    private void SetupButtonListeners()
    {
        // Lobby buttons
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(ShowLeaderboard);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        if (signOutButton != null)
            signOutButton.onClick.AddListener(SignOut);
        
        // Game panel buttons
        if (backToLobbyButton != null)
            backToLobbyButton.onClick.AddListener(BackToLobby);
            
        if (submitScoreButton != null)
            submitScoreButton.onClick.AddListener(SubmitScore);
        
        // Leaderboard buttons
        if (refreshLeaderboardButton != null)
            refreshLeaderboardButton.onClick.AddListener(RefreshLeaderboard);
            
        if (closeLeaderboardButton != null)
            closeLeaderboardButton.onClick.AddListener(CloseLeaderboard);
        
        // Settings buttons
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
    }
    
    private void InitializeUI()
    {
        // Show lobby by default
        ShowPanel(lobbyPanel);
        
        // Initialize settings
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        if (soundToggle != null)
        {
            soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
            soundToggle.onValueChanged.AddListener(OnSoundToggle);
        }
        
        if (musicToggle != null)
        {
            musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            musicToggle.onValueChanged.AddListener(OnMusicToggle);
        }
    }
    
    private void UpdatePlayerInfo()
    {
        if (playerNameText != null)
        {
            string uid = firebaseManager?.GetCurrentUserUID();
            playerNameText.text = string.IsNullOrEmpty(uid) ? "Guest" : $"Player: {uid.Substring(0, 8)}...";
        }
        
        if (playerScoreText != null)
        {
            // This would be updated from server data
            playerScoreText.text = "Score: 0";
        }
    }
    
    private void UpdateGameScore()
    {
        if (gameScoreText != null && gameManager != null)
        {
            gameScoreText.text = $"Score: {gameManager.currentScore}";
        }
    }
    
    public void ShowPanel(GameObject panel)
    {
        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Show target panel
        if (panel != null) panel.SetActive(true);
    }
    
    public void ShowLeaderboard()
    {
        ShowPanel(leaderboardPanel);
        RefreshLeaderboard();
    }
    
    public void ShowSettings()
    {
        ShowPanel(settingsPanel);
    }
    
    public void BackToLobby()
    {
        ShowPanel(lobbyPanel);
        if (gameManager != null)
            gameManager.BackToLobby();
    }
    
    public void SubmitScore()
    {
        if (gameManager != null)
            gameManager.SubmitScore();
    }
    
    public void SignOut()
    {
        if (firebaseManager != null)
            firebaseManager.SignOut();
            
        // Reload scene or show main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void RefreshLeaderboard()
    {
        StartCoroutine(FetchAndDisplayLeaderboard());
    }
    
    private IEnumerator FetchAndDisplayLeaderboard()
    {
        // Clear existing leaderboard items
        if (leaderboardContent != null)
        {
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Fetch leaderboard data
        if (gameManager != null)
        {
            gameManager.LoadLeaderboard();
        }
        
        // Simulate leaderboard data (replace with actual API call)
        yield return new WaitForSeconds(1f);
        
        // Create sample leaderboard items
        for (int i = 0; i < 10; i++)
        {
            CreateLeaderboardItem($"Player {i + 1}", Random.Range(100, 2000), i + 1);
        }
    }
    
    private void CreateLeaderboardItem(string playerName, int score, int rank)
    {
        if (leaderboardItemPrefab != null && leaderboardContent != null)
        {
            GameObject item = Instantiate(leaderboardItemPrefab, leaderboardContent);
            
            // Setup item text
            Text[] texts = item.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"#{rank}";
                texts[1].text = playerName;
                texts[2].text = score.ToString();
            }
        }
    }
    
    public void CloseLeaderboard()
    {
        ShowPanel(lobbyPanel);
    }
    
    public void CloseSettings()
    {
        ShowPanel(lobbyPanel);
    }
    
    private void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        AudioListener.volume = value;
    }
    
    private void OnSoundToggle(bool enabled)
    {
        PlayerPrefs.SetInt("SoundEnabled", enabled ? 1 : 0);
        // Apply sound settings
    }
    
    private void OnMusicToggle(bool enabled)
    {
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        // Apply music settings
    }
    
    // Public methods for external calls
    public void ShowGamePanel(string gameType)
    {
        ShowPanel(gamePanel);
        if (gameTitleText != null)
            gameTitleText.text = $"{gameType} Game";
    }
    
    public void UpdateProgress(float progress)
    {
        if (progressSlider != null)
            progressSlider.value = progress;
    }
}
