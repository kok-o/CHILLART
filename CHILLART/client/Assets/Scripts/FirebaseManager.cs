using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase Settings")]
    public bool useAnonymousAuth = true;
    
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private bool isInitialized = false;
    
    private static FirebaseManager _instance;
    public static FirebaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FirebaseManager>();
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
        StartCoroutine(InitializeFirebase());
    }
    
    private IEnumerator InitializeFirebase()
    {
        // Check if Firebase is available
        var checkTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);
        
        var dependencyStatus = checkTask.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase dependencies are available");
            InitializeAuth();
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
        }
    }
    
    private void InitializeAuth()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        
        if (useAnonymousAuth)
        {
            StartCoroutine(SignInAnonymously());
        }
    }
    
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != currentUser)
        {
            bool signedIn = currentUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && currentUser != null)
            {
                Debug.Log("Signed out " + currentUser.UserId);
            }
            currentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + currentUser.UserId);
                isInitialized = true;
            }
        }
    }
    
    private IEnumerator SignInAnonymously()
    {
        var signInTask = auth.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => signInTask.IsCompleted);
        
        if (signInTask.Exception != null)
        {
            Debug.LogError($"Failed to sign in anonymously: {signInTask.Exception}");
        }
        else
        {
            Debug.Log("Successfully signed in anonymously");
        }
    }
    
    public string GetCurrentUserUID()
    {
        if (currentUser != null)
        {
            return currentUser.UserId;
        }
        return null;
    }
    
    public bool IsUserSignedIn()
    {
        return currentUser != null;
    }
    
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    public async Task<bool> SignInWithEmailAsync(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Signed in with email: {result.User.Email}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to sign in with email: {e.Message}");
            return false;
        }
    }
    
    public async Task<bool> CreateUserWithEmailAsync(string email, string password)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Created user with email: {result.User.Email}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create user with email: {e.Message}");
            return false;
        }
    }
    
    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("User signed out");
        }
    }
    
    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }
}
