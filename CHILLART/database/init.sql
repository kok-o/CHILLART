-- Game Club Database Initialization Script
-- PostgreSQL Database Schema

-- Create database (run this manually if needed)
-- CREATE DATABASE game_club;

-- Connect to the database
-- \c game_club;

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    firebase_uid VARCHAR(255) UNIQUE NOT NULL,
    total_points INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create scores table
CREATE TABLE IF NOT EXISTS scores (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    points INTEGER NOT NULL,
    game_type VARCHAR(50) DEFAULT 'unknown',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_firebase_uid ON users(firebase_uid);
CREATE INDEX IF NOT EXISTS idx_scores_user_id ON scores(user_id);
CREATE INDEX IF NOT EXISTS idx_scores_created_at ON scores(created_at);
CREATE INDEX IF NOT EXISTS idx_users_total_points ON users(total_points DESC);

-- Create function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create trigger to automatically update updated_at
CREATE TRIGGER update_users_updated_at 
    BEFORE UPDATE ON users 
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- Insert some sample data for testing
INSERT INTO users (firebase_uid, total_points) VALUES 
    ('test-user-1', 1500),
    ('test-user-2', 1200),
    ('test-user-3', 800)
ON CONFLICT (firebase_uid) DO NOTHING;

-- Insert sample scores
INSERT INTO scores (user_id, points, game_type) VALUES 
    (1, 500, 'puzzle'),
    (1, 300, 'memory'),
    (1, 700, 'arcade'),
    (2, 400, 'puzzle'),
    (2, 800, 'memory'),
    (3, 200, 'puzzle'),
    (3, 600, 'arcade')
ON CONFLICT DO NOTHING;

-- Update total points for sample users
UPDATE users SET total_points = (
    SELECT COALESCE(SUM(points), 0) 
    FROM scores 
    WHERE user_id = users.id
);

-- Create view for leaderboard
CREATE OR REPLACE VIEW leaderboard_view AS
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

-- Grant permissions (adjust as needed for your setup)
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO your_user;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO your_user;
