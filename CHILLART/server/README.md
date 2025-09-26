# Game Club Server

Node.js Express server for the Game Club MVP project.

## 🚀 Quick Start

### Prerequisites
- Node.js 18+
- PostgreSQL 14+
- npm or yarn

### Installation
```bash
# Install dependencies
npm install

# Copy environment variables
cp env.example .env

# Edit .env file with your database settings
# DB_HOST=localhost
# DB_PORT=5432
# DB_NAME=game_club
# DB_USER=postgres
# DB_PASSWORD=your_password

# Start development server
npm run dev

# Start production server
npm start
```

## 📡 API Endpoints

### Health Check
```
GET /health
```
Returns server status and timestamp.

### Submit Score
```
POST /score
Content-Type: application/json

{
  "uid": "firebase-uid",
  "points": 120,
  "gameType": "puzzle"
}
```

### Get Leaderboard
```
GET /leaderboard?limit=10
```
Returns top players with their scores.

### Get User Stats
```
GET /user/:uid/stats
```
Returns detailed statistics for a specific user.

## 🗄️ Database Schema

### Users Table
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    firebase_uid VARCHAR(255) UNIQUE NOT NULL,
    total_points INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

### Scores Table
```sql
CREATE TABLE scores (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    points INTEGER NOT NULL,
    game_type VARCHAR(50) DEFAULT 'unknown',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

## 🔧 Configuration

### Environment Variables
- `DB_HOST` - Database host (default: localhost)
- `DB_PORT` - Database port (default: 5432)
- `DB_NAME` - Database name (default: game_club)
- `DB_USER` - Database user (default: postgres)
- `DB_PASSWORD` - Database password
- `PORT` - Server port (default: 3000)
- `NODE_ENV` - Environment (development/production)

### Firebase Configuration (Optional)
- `FIREBASE_PROJECT_ID` - Firebase project ID
- `FIREBASE_PRIVATE_KEY` - Firebase private key
- `FIREBASE_CLIENT_EMAIL` - Firebase client email

## 🧪 Testing

### Manual Testing
```bash
# Health check
curl http://localhost:3000/health

# Submit score
curl -X POST http://localhost:3000/score \
  -H "Content-Type: application/json" \
  -d '{"uid":"test-user","points":100,"gameType":"puzzle"}'

# Get leaderboard
curl http://localhost:3000/leaderboard

# Get user stats
curl http://localhost:3000/user/test-user/stats
```

### Automated Testing
```bash
# Run tests
npm test

# Run tests with coverage
npm run test:coverage

# Run linting
npm run lint
```

## 🚀 Deployment

### Railway
```bash
# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# Deploy
railway up
```

### Render
1. Connect GitHub repository
2. Set environment variables
3. Deploy automatically

### Docker
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
EXPOSE 3000
CMD ["npm", "start"]
```

## 📊 Monitoring

### Logs
- Application logs: `console.log`
- Error logs: `console.error`
- Request logs: Morgan middleware

### Metrics
- Response time
- Memory usage
- Database connections
- Error rates

## 🔒 Security

### Input Validation
- JSON schema validation
- SQL injection prevention
- XSS protection

### Rate Limiting
- Request rate limiting
- IP-based blocking
- User-based limits

### CORS
- Configured for Unity client
- Environment-specific settings
- Preflight request handling

## 🐛 Troubleshooting

### Common Issues

#### Database Connection Failed
- Check PostgreSQL service status
- Verify connection credentials
- Check network connectivity
- Review firewall settings

#### Port Already in Use
- Change PORT in .env file
- Kill existing process
- Use different port

#### Firebase Authentication Issues
- Verify Firebase configuration
- Check project settings
- Review security rules

### Debug Mode
```bash
# Enable debug logging
DEBUG=* npm run dev

# Enable specific debug modules
DEBUG=express:* npm run dev
```

## 📈 Performance

### Optimization Tips
- Use connection pooling
- Implement caching
- Optimize database queries
- Use compression middleware

### Load Testing
```bash
# Install artillery
npm install -g artillery

# Run load test
artillery run load-test.yml
```

## 🔄 Updates

### Database Migrations
```bash
# Run migration
psql -U postgres -d game_club -f database/migrations/001_create_tables.sql

# Rollback migration
psql -U postgres -d game_club -f database/migrations/rollback/001_rollback.sql
```

### Version Updates
- Check Node.js compatibility
- Update dependencies
- Test thoroughly
- Deploy gradually

## 📚 Documentation

### API Documentation
- Swagger/OpenAPI specification
- Postman collection
- Example requests/responses

### Code Documentation
- JSDoc comments
- README files
- Architecture diagrams

## 🤝 Contributing

### Development Process
1. Fork repository
2. Create feature branch
3. Make changes
4. Add tests
5. Submit pull request

### Code Standards
- ESLint configuration
- Prettier formatting
- Conventional commits
- Test coverage requirements

## 📞 Support

### Getting Help
- Check documentation
- Search existing issues
- Create new issue
- Contact development team

### Reporting Bugs
- Include error messages
- Provide reproduction steps
- Share environment details
- Attach relevant logs
