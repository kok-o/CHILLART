using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class ScoreData
{
    public string uid;
    public int points;
    public string gameType;
}

[System.Serializable]
public class LeaderboardItem
{
    public string firebase_uid;
    public int total_points;
    public string user_created_at;
    public int games_played;
}

[System.Serializable]
public class LeaderboardResponse
{
    public bool success;
    public List<LeaderboardItem> leaderboard;
    public int total;
}

[System.Serializable]
public class UserStats
{
    public string firebase_uid;
    public int total_points;
    public string user_created_at;
    public int games_played;
    public float average_score;
    public int best_score;
}

[System.Serializable]
public class UserStatsResponse
{
    public bool success;
    public UserStats stats;
}

public class APIManager : MonoBehaviour
{
    [Header("Server Configuration")]
    public string serverUrl = "http://localhost:3000";
    public float requestTimeout = 10f;
    
    private static APIManager _instance;
    public static APIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<APIManager>();
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
    
    // Submit score to server
    public IEnumerator SubmitScore(string uid, int points, string gameType, System.Action<bool, string> callback)
    {
        var scoreData = new ScoreData
        {
            uid = uid,
            points = points,
            gameType = gameType
        };
        
        string jsonData = JsonUtility.ToJson(scoreData);
        
        using (var request = new UnityWebRequest($"{serverUrl}/score", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)requestTimeout;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted successfully: " + request.downloadHandler.text);
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to submit score: " + request.error);
                callback?.Invoke(false, request.error);
            }
        }
    }
    
    // Fetch leaderboard from server
    public IEnumerator FetchLeaderboard(int limit, System.Action<bool, LeaderboardResponse> callback)
    {
        using (var request = UnityWebRequest.Get($"{serverUrl}/leaderboard?limit={limit}"))
        {
            request.timeout = (int)requestTimeout;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    LeaderboardResponse leaderboardData = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);
                    callback?.Invoke(true, leaderboardData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to parse leaderboard response: " + e.Message);
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch leaderboard: " + request.error);
                callback?.Invoke(false, null);
            }
        }
    }
    
    // Fetch user statistics
    public IEnumerator FetchUserStats(string uid, System.Action<bool, UserStatsResponse> callback)
    {
        using (var request = UnityWebRequest.Get($"{serverUrl}/user/{uid}/stats"))
        {
            request.timeout = (int)requestTimeout;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    UserStatsResponse userStats = JsonUtility.FromJson<UserStatsResponse>(jsonResponse);
                    callback?.Invoke(true, userStats);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to parse user stats response: " + e.Message);
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch user stats: " + request.error);
                callback?.Invoke(false, null);
            }
        }
    }
    
    // Health check
    public IEnumerator HealthCheck(System.Action<bool, string> callback)
    {
        using (var request = UnityWebRequest.Get($"{serverUrl}/health"))
        {
            request.timeout = (int)requestTimeout;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                callback?.Invoke(false, request.error);
            }
        }
    }
    
    // Helper method to start coroutines from other scripts
    public void SubmitScoreAsync(string uid, int points, string gameType, System.Action<bool, string> callback)
    {
        StartCoroutine(SubmitScore(uid, points, gameType, callback));
    }
    
    public void FetchLeaderboardAsync(int limit, System.Action<bool, LeaderboardResponse> callback)
    {
        StartCoroutine(FetchLeaderboard(limit, callback));
    }
    
    public void FetchUserStatsAsync(string uid, System.Action<bool, UserStatsResponse> callback)
    {
        StartCoroutine(FetchUserStats(uid, callback));
    }
    
    public void HealthCheckAsync(System.Action<bool, string> callback)
    {
        StartCoroutine(HealthCheck(callback));
    }
}
