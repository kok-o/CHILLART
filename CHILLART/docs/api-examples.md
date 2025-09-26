# API Examples

## 🧪 Testing API Endpoints

### 1. Health Check
```bash
curl -X GET http://localhost:3000/health
```

**Response:**
```json
{
  "status": "OK",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

### 2. Submit Score
```bash
curl -X POST http://localhost:3000/score \
  -H "Content-Type: application/json" \
  -d '{
    "uid": "firebase-user-123",
    "points": 150,
    "gameType": "puzzle"
  }'
```

**Response:**
```json
{
  "success": true,
  "score": {
    "id": 1,
    "user_id": 1,
    "points": 150,
    "game_type": "puzzle",
    "created_at": "2024-01-15T10:30:00.000Z"
  },
  "message": "Score saved successfully"
}
```

### 3. Get Leaderboard
```bash
curl -X GET "http://localhost:3000/leaderboard?limit=5"
```

**Response:**
```json
{
  "success": true,
  "leaderboard": [
    {
      "firebase_uid": "firebase-user-123",
      "total_points": 1500,
      "user_created_at": "2024-01-15T10:00:00.000Z",
      "games_played": 10
    },
    {
      "firebase_uid": "firebase-user-456",
      "total_points": 1200,
      "user_created_at": "2024-01-15T09:00:00.000Z",
      "games_played": 8
    }
  ],
  "total": 2
}
```

### 4. Get User Stats
```bash
curl -X GET http://localhost:3000/user/firebase-user-123/stats
```

**Response:**
```json
{
  "success": true,
  "stats": {
    "firebase_uid": "firebase-user-123",
    "total_points": 1500,
    "user_created_at": "2024-01-15T10:00:00.000Z",
    "games_played": 10,
    "average_score": 150.0,
    "best_score": 300
  }
}
```

## 🎮 Unity Integration Examples

### C# Code Examples

#### Submit Score from Unity
```csharp
public class ScoreManager : MonoBehaviour
{
    private string serverUrl = "http://localhost:3000";
    
    public void SubmitScore(int points, string gameType)
    {
        StartCoroutine(SubmitScoreCoroutine(points, gameType));
    }
    
    private IEnumerator SubmitScoreCoroutine(int points, string gameType)
    {
        string uid = FirebaseManager.Instance.GetCurrentUserUID();
        
        var scoreData = new
        {
            uid = uid,
            points = points,
            gameType = gameType
        };
        
        string jsonData = JsonUtility.ToJson(scoreData);
        
        using (var request = new UnityWebRequest($"{serverUrl}/score", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to submit score: " + request.error);
            }
        }
    }
}
```

#### Fetch Leaderboard from Unity
```csharp
public class LeaderboardManager : MonoBehaviour
{
    private string serverUrl = "http://localhost:3000";
    
    public void LoadLeaderboard()
    {
        StartCoroutine(LoadLeaderboardCoroutine());
    }
    
    private IEnumerator LoadLeaderboardCoroutine()
    {
        using (var request = UnityWebRequest.Get($"{serverUrl}/leaderboard?limit=10"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                LeaderboardResponse leaderboard = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);
                
                // Display leaderboard in UI
                DisplayLeaderboard(leaderboard.leaderboard);
            }
            else
            {
                Debug.LogError("Failed to load leaderboard: " + request.error);
            }
        }
    }
    
    private void DisplayLeaderboard(List<LeaderboardItem> items)
    {
        // Update UI with leaderboard data
        foreach (var item in items)
        {
            Debug.Log($"Player: {item.firebase_uid}, Score: {item.total_points}");
        }
    }
}
```

## 🔧 JavaScript/Node.js Examples

### Server-side Score Validation
```javascript
// Middleware for score validation
const validateScore = (req, res, next) => {
    const { uid, points, gameType } = req.body;
    
    // Validate required fields
    if (!uid || !points) {
        return res.status(400).json({
            error: 'Missing required fields: uid and points'
        });
    }
    
    // Validate points range
    if (points < 0 || points > 10000) {
        return res.status(400).json({
            error: 'Points must be between 0 and 10000'
        });
    }
    
    // Validate game type
    const validGameTypes = ['puzzle', 'memory', 'arcade'];
    if (gameType && !validGameTypes.includes(gameType)) {
        return res.status(400).json({
            error: 'Invalid game type'
        });
    }
    
    next();
};

// Apply middleware to score endpoint
app.post('/score', validateScore, async (req, res) => {
    // Score submission logic
});
```

### Database Query Examples
```javascript
// Get top 10 players with their best scores
const getTopPlayers = async (limit = 10) => {
    const query = `
        SELECT 
            u.firebase_uid,
            u.total_points,
            MAX(s.points) as best_score,
            COUNT(s.id) as games_played
        FROM users u
        LEFT JOIN scores s ON u.id = s.user_id
        GROUP BY u.id, u.firebase_uid, u.total_points
        ORDER BY u.total_points DESC
        LIMIT $1
    `;
    
    const result = await pool.query(query, [limit]);
    return result.rows;
};

// Get user's game history
const getUserGameHistory = async (uid, limit = 20) => {
    const query = `
        SELECT 
            s.points,
            s.game_type,
            s.created_at
        FROM users u
        JOIN scores s ON u.id = s.user_id
        WHERE u.firebase_uid = $1
        ORDER BY s.created_at DESC
        LIMIT $2
    `;
    
    const result = await pool.query(query, [uid, limit]);
    return result.rows;
};
```

## 🧪 Testing with Postman

### Postman Collection
```json
{
  "info": {
    "name": "Game Club API",
    "description": "API endpoints for Game Club MVP"
  },
  "item": [
    {
      "name": "Health Check",
      "request": {
        "method": "GET",
        "header": [],
        "url": {
          "raw": "http://localhost:3000/health",
          "protocol": "http",
          "host": ["localhost"],
          "port": "3000",
          "path": ["health"]
        }
      }
    },
    {
      "name": "Submit Score",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"uid\": \"test-user-123\",\n  \"points\": 150,\n  \"gameType\": \"puzzle\"\n}"
        },
        "url": {
          "raw": "http://localhost:3000/score",
          "protocol": "http",
          "host": ["localhost"],
          "port": "3000",
          "path": ["score"]
        }
      }
    }
  ]
}
```

## 🚀 Performance Testing

### Load Testing with Artillery
```yaml
# artillery-load-test.yml
config:
  target: 'http://localhost:3000'
  phases:
    - duration: 60
      arrivalRate: 10
  defaults:
    headers:
      Content-Type: 'application/json'

scenarios:
  - name: "Submit Score"
    weight: 70
    flow:
      - post:
          url: "/score"
          json:
            uid: "load-test-user-{{ $randomInt(1, 1000) }}"
            points: "{{ $randomInt(50, 500) }}"
            gameType: "puzzle"
  
  - name: "Get Leaderboard"
    weight: 30
    flow:
      - get:
          url: "/leaderboard?limit=10"
```

### Run Load Test
```bash
# Install Artillery
npm install -g artillery

# Run load test
artillery run artillery-load-test.yml

# Run with report
artillery run artillery-load-test.yml --output report.json
artillery report report.json
```

## 🔍 Error Handling Examples

### Common Error Responses
```json
// 400 Bad Request
{
  "error": "Missing required fields: uid and points"
}

// 404 Not Found
{
  "error": "User not found"
}

// 500 Internal Server Error
{
  "error": "Internal server error",
  "message": "Database connection failed"
}
```

### Unity Error Handling
```csharp
private IEnumerator HandleAPIError(UnityWebRequest request)
{
    if (request.result != UnityWebRequest.Result.Success)
    {
        string errorMessage = "Unknown error";
        
        try
        {
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            errorMessage = errorResponse.error;
        }
        catch
        {
            errorMessage = request.error;
        }
        
        // Show error to user
        ShowErrorMessage(errorMessage);
        
        // Log error for debugging
        Debug.LogError($"API Error: {errorMessage}");
    }
}
```
