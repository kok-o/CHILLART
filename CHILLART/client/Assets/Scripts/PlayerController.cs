using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float teleportSpeed = 10f;
    
    [Header("Animation")]
    public Animator animator;
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isTeleporting = false;
    
    void Start()
    {
        // Initialize at starting position
        targetPosition = transform.position;
    }
    
    void Update()
    {
        HandleInput();
        HandleMovement();
    }
    
    private void HandleInput()
    {
        // Handle mouse click for teleportation
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Keep on 2D plane
            
            TeleportTo(mouseWorldPos);
        }
        
        // Handle keyboard movement (optional)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            Move(Vector3.up);
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            Move(Vector3.down);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            Move(Vector3.left);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            Move(Vector3.right);
    }
    
    private void HandleMovement()
    {
        if (isTeleporting)
        {
            // Smooth teleportation
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isTeleporting = false;
                isMoving = false;
                
                if (animator != null)
                    animator.SetBool("IsMoving", false);
            }
        }
        else if (isMoving)
        {
            // Regular movement
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
                
                if (animator != null)
                    animator.SetBool("IsMoving", false);
            }
        }
    }
    
    public void TeleportTo(Vector3 position)
    {
        targetPosition = position;
        isTeleporting = true;
        isMoving = false;
        
        if (animator != null)
            animator.SetBool("IsMoving", true);
            
        Debug.Log($"Teleporting to: {position}");
    }
    
    public void Move(Vector3 direction)
    {
        targetPosition = transform.position + direction;
        isMoving = true;
        isTeleporting = false;
        
        if (animator != null)
            animator.SetBool("IsMoving", true);
    }
    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        targetPosition = position;
        isMoving = false;
        isTeleporting = false;
        
        if (animator != null)
            animator.SetBool("IsMoving", false);
    }
    
    // Called when player reaches a game machine
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GameMachine"))
        {
            GameMachine machine = other.GetComponent<GameMachine>();
            if (machine != null)
            {
                Debug.Log($"Player reached {machine.gameType} machine");
                // Machine interaction is handled by the machine itself
            }
        }
    }
    
    // Visual feedback for movement
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
