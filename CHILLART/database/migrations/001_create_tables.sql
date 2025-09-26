-- Migration 001: Create initial tables
-- Run this if you need to recreate tables

-- Drop existing tables if they exist (in correct order due to foreign keys)
DROP TABLE IF EXISTS scores CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP VIEW IF EXISTS leaderboard_view CASCADE;
DROP FUNCTION IF EXISTS update_updated_at_column() CASCADE;

-- Create users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    firebase_uid VARCHAR(255) UNIQUE NOT NULL,
    total_points INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create scores table
CREATE TABLE scores (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    points INTEGER NOT NULL,
    game_type VARCHAR(50) DEFAULT 'unknown',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create indexes
CREATE INDEX idx_users_firebase_uid ON users(firebase_uid);
CREATE INDEX idx_scores_user_id ON scores(user_id);
CREATE INDEX idx_scores_created_at ON scores(created_at);
CREATE INDEX idx_users_total_points ON users(total_points DESC);

-- Create function and trigger for updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_updated_at 
    BEFORE UPDATE ON users 
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- Create leaderboard view
CREATE VIEW leaderboard_view AS
SELECT 
    u.id,
    u.firebase_uid,
    u.total_points,
    u.created_at as user_created_at,
    COUNT(s.id) as games_played,
    COALESCE(AVG(s.points), 0) as average_score,
    COALESCE(MAX(s.points), 0) as best_score
FROM users u
LEFT JOIN scores s ON u.id = s.user_id
GROUP BY u.id, u.firebase_uid, u.total_points, u.created_at
ORDER BY u.total_points DESC, u.created_at ASC;
