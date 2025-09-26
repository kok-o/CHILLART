# Unity 2D Game Club Client

## Project Structure

```
Assets/
├── Scenes/
│   └── LobbyScene.unity
├── Scripts/
│   ├── GameManager.cs
│   ├── PlayerController.cs
│   ├── GameMachine.cs
│   ├── UIManager.cs
│   └── FirebaseManager.cs
├── Sprites/
│   ├── Background/
│   ├── Machines/
│   └── UI/
├── Prefabs/
│   ├── GameMachine.prefab
│   └── Player.prefab
└── Firebase/
    └── google-services.json (Android)
    └── GoogleService-Info.plist (iOS)
```

## Setup Instructions

### 1. Unity Version
- Unity 2022.3 LTS or newer
- 2D Template

### 2. Firebase Setup
1. Create Firebase project at [console.firebase.google.com](https://console.firebase.google.com)
2. Enable Authentication (Anonymous sign-in)
3. Download config files:
   - `google-services.json` for Android
   - `GoogleService-Info.plist` for iOS
4. Place in `Assets/Firebase/` folder

### 3. Firebase SDK
1. Open Unity Package Manager
2. Add package from Git URL: `https://github.com/firebase/quickstart-unity.git`
3. Or download from [Firebase Unity SDK](https://firebase.google.com/download/unity)

### 4. Scene Setup
- Create LobbyScene with 2D background
- Add 3 GameMachine prefabs (Puzzle, Memory, Arcade)
- Add Player prefab
- Setup UI Canvas with buttons

## Features

### LobbyScene
- 2D game hall background
- 3 clickable game machines
- Player teleportation on click
- UI panel for game selection

### Game Machines
- Puzzle Machine
- Memory Machine  
- Arcade Machine

### Player System
- Click-to-move teleportation
- Firebase UID integration
- Score tracking

## API Integration

The client communicates with the Node.js server:
- `POST /score` - Submit game scores
- `GET /leaderboard` - Fetch top players
- `GET /user/:uid/stats` - Get user statistics

## Build Settings

### Android
- Minimum API Level: 21
- Target API Level: 33
- Architecture: ARM64

### WebGL
- Template: Default
- Compression Format: Gzip
