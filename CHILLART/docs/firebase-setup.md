# Firebase Setup Guide

## 1. Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com)
2. Click "Create a project"
3. Enter project name: "Game Club MVP"
4. Enable Google Analytics (optional)
5. Create project

## 2. Enable Authentication

1. In Firebase Console, go to "Authentication"
2. Click "Get started"
3. Go to "Sign-in method" tab
4. Enable "Anonymous" authentication
5. Optionally enable "Email/Password" for future use

## 3. Download Configuration Files

### For Android:
1. Go to Project Settings (gear icon)
2. Select "Your apps" tab
3. Click "Add app" → Android
4. Enter package name: `com.yourcompany.gameclub`
5. Download `google-services.json`
6. Place in `client/Assets/Firebase/`

### For iOS:
1. Click "Add app" → iOS
2. Enter bundle ID: `com.yourcompany.gameclub`
3. Download `GoogleService-Info.plist`
4. Place in `client/Assets/Firebase/`

### For Web:
1. Click "Add app" → Web
2. Enter app nickname
3. Copy the config object
4. Use in Unity WebGL builds

## 4. Unity Firebase SDK Setup

### Method 1: Unity Package Manager
1. Open Unity Package Manager
2. Click "+" → "Add package from git URL"
3. Enter: `https://github.com/firebase/quickstart-unity.git`
4. Click "Add"

### Method 2: Manual Download
1. Download Firebase Unity SDK from [Firebase Downloads](https://firebase.google.com/download/unity)
2. Import the `.unitypackage` file
3. Follow the setup wizard

## 5. Firebase Configuration in Unity

### Android Setup:
1. Place `google-services.json` in `Assets/Firebase/`
2. Unity will automatically detect and configure

### iOS Setup:
1. Place `GoogleService-Info.plist` in `Assets/Firebase/`
2. Unity will automatically detect and configure

### WebGL Setup:
1. Use Firebase Web SDK configuration
2. Add config to `FirebaseManager.cs`

## 6. Testing Authentication

```csharp
// Test anonymous sign-in
FirebaseManager.Instance.SignInAnonymously();

// Check if user is signed in
bool isSignedIn = FirebaseManager.Instance.IsUserSignedIn();

// Get user UID
string uid = FirebaseManager.Instance.GetCurrentUserUID();
```

## 7. Security Rules (Optional)

For production, consider adding security rules:

```javascript
// Firestore Rules (if using Firestore)
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /users/{userId} {
      allow read, write: if request.auth != null && request.auth.uid == userId;
    }
  }
}
```

## 8. Troubleshooting

### Common Issues:

1. **Dependencies not resolved**
   - Check Unity version compatibility
   - Update Firebase SDK
   - Clear Unity cache

2. **Authentication fails**
   - Check internet connection
   - Verify Firebase project settings
   - Check console for error messages

3. **Build errors**
   - Ensure config files are in correct location
   - Check build settings
   - Verify package names match

### Debug Tips:

1. Enable Firebase debug logging
2. Check Unity Console for errors
3. Use Firebase Console to monitor authentication
4. Test on different platforms

## 9. Production Considerations

1. **Security**
   - Use proper authentication methods
   - Implement server-side validation
   - Use Firebase Security Rules

2. **Performance**
   - Cache authentication state
   - Handle network errors gracefully
   - Implement retry logic

3. **Analytics**
   - Track user engagement
   - Monitor authentication success rates
   - Set up crash reporting
