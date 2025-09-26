const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const morgan = require('morgan');
const { Pool } = require('pg');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(helmet());
app.use(cors());
app.use(morgan('combined'));
app.use(express.json());

// PostgreSQL connection
const pool = new Pool({
    user: process.env.DB_USER || 'postgres',
    host: process.env.DB_HOST || 'localhost',
    database: process.env.DB_NAME || 'game_club',
    password: process.env.DB_PASSWORD || 'password',
    port: process.env.DB_PORT || 5432,
});

// Test database connection
pool.on('connect', () => {
    console.log('Connected to PostgreSQL database');
});

pool.on('error', (err) => {
    console.error('PostgreSQL connection error:', err);
});

// Routes

// Health check
app.get('/health', (req, res) => {
    res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

// Save player score
app.post('/score', async(req, res) => {
    try {
        const { uid, points, gameType } = req.body;

        if (!uid || !points) {
            return res.status(400).json({
                error: 'Missing required fields: uid and points'
            });
        }

        // Check if user exists, if not create one
        const userCheckQuery = 'SELECT id FROM users WHERE firebase_uid = $1';
        const userResult = await pool.query(userCheckQuery, [uid]);

        let userId;
        if (userResult.rows.length === 0) {
            // Create new user
            const createUserQuery = 'INSERT INTO users (firebase_uid, created_at) VALUES ($1, NOW()) RETURNING id';
            const newUserResult = await pool.query(createUserQuery, [uid]);
            userId = newUserResult.rows[0].id;
        } else {
            userId = userResult.rows[0].id;
        }

        // Save score
        const scoreQuery = `
      INSERT INTO scores (user_id, points, game_type, created_at) 
      VALUES ($1, $2, $3, NOW()) 
      RETURNING *
    `;
        const scoreResult = await pool.query(scoreQuery, [userId, points, gameType || 'unknown']);

        // Update user's total points
        const updateTotalQuery = `
      UPDATE users 
      SET total_points = (
        SELECT COALESCE(SUM(points), 0) 
        FROM scores 
        WHERE user_id = $1
      )
      WHERE id = $1
    `;
        await pool.query(updateTotalQuery, [userId]);

        res.json({
            success: true,
            score: scoreResult.rows[0],
            message: 'Score saved successfully'
        });

    } catch (error) {
        console.error('Error saving score:', error);
        res.status(500).json({
            error: 'Internal server error',
            message: error.message
        });
    }
});

// Get leaderboard
app.get('/leaderboard', async(req, res) => {
    try {
        const limit = parseInt(req.query.limit) || 10;

        const leaderboardQuery = `
      SELECT 
        u.firebase_uid,
        u.total_points,
        u.created_at as user_created_at,
        COUNT(s.id) as games_played
      FROM users u
      LEFT JOIN scores s ON u.id = s.user_id
      GROUP BY u.id, u.firebase_uid, u.total_points, u.created_at
      ORDER BY u.total_points DESC, u.created_at ASC
      LIMIT $1
    `;

        const result = await pool.query(leaderboardQuery, [limit]);

        res.json({
            success: true,
            leaderboard: result.rows,
            total: result.rows.length
        });

    } catch (error) {
        console.error('Error fetching leaderboard:', error);
        res.status(500).json({
            error: 'Internal server error',
            message: error.message
        });
    }
});

// Get user stats
app.get('/user/:uid/stats', async(req, res) => {
    try {
        const { uid } = req.params;

        const userStatsQuery = `
      SELECT 
        u.firebase_uid,
        u.total_points,
        u.created_at as user_created_at,
        COUNT(s.id) as games_played,
        AVG(s.points) as average_score,
        MAX(s.points) as best_score
      FROM users u
      LEFT JOIN scores s ON u.id = s.user_id
      WHERE u.firebase_uid = $1
      GROUP BY u.id, u.firebase_uid, u.total_points, u.created_at
    `;

        const result = await pool.query(userStatsQuery, [uid]);

        if (result.rows.length === 0) {
            return res.status(404).json({
                error: 'User not found'
            });
        }

        res.json({
            success: true,
            stats: result.rows[0]
        });

    } catch (error) {
        console.error('Error fetching user stats:', error);
        res.status(500).json({
            error: 'Internal server error',
            message: error.message
        });
    }
});

// Error handling middleware
app.use((err, req, res, next) => {
    console.error(err.stack);
    res.status(500).json({
        error: 'Something went wrong!',
        message: err.message
    });
});

// 404 handler
app.use('*', (req, res) => {
    res.status(404).json({
        error: 'Route not found'
    });
});

// Start server
app.listen(PORT, () => {
    console.log(`Game Club Server running on port ${PORT}`);
    console.log(`Health check: http://localhost:${PORT}/health`);
});