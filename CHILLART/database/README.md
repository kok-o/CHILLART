# Database Setup

## PostgreSQL Setup

### 1. Install PostgreSQL
- Download and install PostgreSQL from [postgresql.org](https://www.postgresql.org/download/)
- Or use Docker: `docker run --name postgres-game-club -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres`

### 2. Create Database
```sql
CREATE DATABASE game_club;
```

### 3. Run Initialization Script
```bash
psql -U postgres -d game_club -f database/init.sql
```

### 4. Environment Variables
Copy `server/env.example` to `server/.env` and configure:
```
DB_HOST=localhost
DB_PORT=5432
DB_NAME=game_club
DB_USER=postgres
DB_PASSWORD=password
```

## Database Schema

### Tables

#### users
- `id` - Primary key
- `firebase_uid` - Firebase user ID (unique)
- `total_points` - Total points earned
- `created_at` - Account creation timestamp
- `updated_at` - Last update timestamp

#### scores
- `id` - Primary key
- `user_id` - Foreign key to users table
- `points` - Points earned in this game
- `game_type` - Type of game (puzzle, memory, arcade)
- `created_at` - Score timestamp

### Views

#### leaderboard_view
Combined view showing user stats and rankings.

## API Endpoints

- `POST /score` - Save player score
- `GET /leaderboard` - Get top players
- `GET /user/:uid/stats` - Get user statistics
- `GET /health` - Health check
