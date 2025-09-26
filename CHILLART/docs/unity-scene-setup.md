# Unity Scene Setup Guide

## LobbyScene Setup

### 1. Create New Scene
1. File → New Scene
2. Choose "2D" template
3. Save as "LobbyScene" in `Assets/Scenes/`

### 2. Scene Hierarchy
```
LobbyScene
├── Main Camera
├── Background
│   └── GameHallBackground (Sprite)
├── GameMachines
│   ├── PuzzleMachine
│   │   ├── MachineSprite (SpriteRenderer)
│   │   ├── MachineCollider (BoxCollider2D)
│   │   └── MachineName (Text)
│   ├── MemoryMachine
│   │   ├── MachineSprite (SpriteRenderer)
│   │   ├── MachineCollider (BoxCollider2D)
│   │   └── MachineName (Text)
│   └── ArcadeMachine
│       ├── MachineSprite (SpriteRenderer)
│       ├── MachineCollider (BoxCollider2D)
│       └── MachineName (Text)
├── Player
│   ├── PlayerSprite (SpriteRenderer)
│   ├── PlayerCollider (CircleCollider2D)
│   └── PlayerController (Script)
├── UI Canvas
│   ├── LobbyPanel
│   │   ├── PlayerInfo
│   │   │   ├── PlayerName (Text)
│   │   │   └── PlayerScore (Text)
│   │   ├── LeaderboardButton (Button)
│   │   ├── SettingsButton (Button)
│   │   └── SignOutButton (Button)
│   ├── GamePanel
│   │   ├── GameTitle (Text)
│   │   ├── GameScore (Text)
│   │   ├── ProgressSlider (Slider)
│   │   ├── BackToLobbyButton (Button)
│   │   └── SubmitScoreButton (Button)
│   ├── LeaderboardPanel
│   │   ├── LeaderboardContent (ScrollView)
│   │   ├── RefreshButton (Button)
│   │   └── CloseButton (Button)
│   └── SettingsPanel
│       ├── VolumeSlider (Slider)
│       ├── SoundToggle (Toggle)
│       ├── MusicToggle (Toggle)
│       └── CloseButton (Button)
└── Managers
    ├── GameManager (Script)
    ├── UIManager (Script)
    ├── FirebaseManager (Script)
    └── APIManager (Script)
```

### 3. Component Setup

#### GameMachine Prefab
1. Create empty GameObject
2. Add components:
   - `SpriteRenderer` (machine sprite)
   - `BoxCollider2D` (interaction area)
   - `GameMachine` script
3. Set tag to "GameMachine"
4. Configure script:
   - `gameType`: "Puzzle", "Memory", or "Arcade"
   - `displayName`: "Puzzle Game", "Memory Game", or "Arcade Game"

#### Player Prefab
1. Create empty GameObject
2. Add components:
   - `SpriteRenderer` (player sprite)
   - `CircleCollider2D` (collision detection)
   - `PlayerController` script
3. Set tag to "Player"
4. Configure script:
   - `moveSpeed`: 5.0
   - `teleportSpeed`: 10.0

#### UI Canvas Setup
1. Create Canvas (Screen Space - Overlay)
2. Add Canvas Scaler (Scale With Screen Size)
3. Set Reference Resolution: 1920x1080
4. Create UI panels as children

### 4. Script Configuration

#### GameManager
- `serverUrl`: "http://localhost:3000"
- Assign UI references
- Set up button listeners

#### UIManager
- Assign all UI panel references
- Configure button listeners
- Set up settings persistence

#### FirebaseManager
- `useAnonymousAuth`: true
- Configure Firebase settings

#### APIManager
- `serverUrl`: "http://localhost:3000"
- `requestTimeout`: 10.0

### 5. Physics Settings
1. Edit → Project Settings → Physics 2D
2. Configure collision matrix
3. Set up trigger detection

### 6. Input Settings
1. Edit → Project Settings → Input Manager
2. Configure mouse input
3. Set up keyboard controls

### 7. Build Settings
1. File → Build Settings
2. Add LobbyScene to build
3. Configure platform settings

## Prefab Creation

### GameMachine Prefab
1. Create prefab from configured GameObject
2. Save in `Assets/Prefabs/`
3. Create variants for each game type

### Player Prefab
1. Create prefab from configured GameObject
2. Save in `Assets/Prefabs/`
3. Configure animation if needed

## Scene Testing

### 1. Play Mode Testing
1. Enter Play Mode
2. Test player movement
3. Test machine interactions
4. Test UI functionality

### 2. Build Testing
1. Build for target platform
2. Test on device
3. Verify Firebase integration
4. Test API connectivity

## Troubleshooting

### Common Issues:
1. **Player not moving**
   - Check PlayerController script
   - Verify input settings
   - Check collision detection

2. **Machines not interactive**
   - Verify GameMachine script
   - Check collider setup
   - Ensure proper tags

3. **UI not responding**
   - Check UIManager script
   - Verify button listeners
   - Check Canvas setup

4. **Firebase errors**
   - Verify configuration files
   - Check internet connection
   - Review console logs

### Debug Tips:
1. Use Unity Console for error messages
2. Add Debug.Log statements
3. Use Scene view for visual debugging
4. Test on different screen resolutions
