using UnityEngine;
using UnityEngine.UI;

public class GameMachine : MonoBehaviour
{
    [Header("Machine Settings")]
    public string gameType = "Unknown";
    public string displayName = "Game Machine";
    
    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color pressedColor = Color.red;
    
    [Header("UI References")]
    public Text machineNameText;
    public Button interactButton;
    
    [Header("Audio")]
    public AudioClip clickSound;
    public AudioClip startGameSound;
    
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isPlayerNearby = false;
    
    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        // Setup UI
        if (machineNameText != null)
            machineNameText.text = displayName;
            
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(StartGame);
            interactButton.gameObject.SetActive(false);
        }
        
        // Set initial color
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }
    
    void Update()
    {
        // Handle keyboard input when player is nearby
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            HighlightMachine(true);
            
            if (interactButton != null)
                interactButton.gameObject.SetActive(true);
                
            Debug.Log($"Player entered {gameType} machine area");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            HighlightMachine(false);
            
            if (interactButton != null)
                interactButton.gameObject.SetActive(false);
                
            Debug.Log($"Player left {gameType} machine area");
        }
    }
    
    void OnMouseDown()
    {
        // Handle mouse click on machine
        if (isPlayerNearby)
        {
            StartGame();
        }
        else
        {
            // Teleport player to machine if clicked from distance
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                Vector3 machinePosition = transform.position;
                machinePosition.y -= 1f; // Position player in front of machine
                player.TeleportTo(machinePosition);
            }
        }
    }
    
    void OnMouseEnter()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = highlightColor;
    }
    
    void OnMouseExit()
    {
        if (spriteRenderer != null && !isPlayerNearby)
            spriteRenderer.color = normalColor;
    }
    
    public void StartGame()
    {
        if (!isPlayerNearby)
        {
            Debug.LogWarning("Player is not nearby the machine");
            return;
        }
        
        // Play sound
        if (audioSource != null && startGameSound != null)
            audioSource.PlayOneShot(startGameSound);
        
        // Visual feedback
        if (spriteRenderer != null)
            spriteRenderer.color = pressedColor;
            
        // Start the game through GameManager
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.StartGame(gameType);
            Debug.Log($"Starting {gameType} game");
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
        
        // Reset color after a short delay
        Invoke(nameof(ResetColor), 0.2f);
    }
    
    private void HighlightMachine(bool highlight)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlight ? highlightColor : normalColor;
        }
    }
    
    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPlayerNearby ? highlightColor : normalColor;
        }
    }
    
    // Visual feedback in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
        
        // Draw interaction area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
