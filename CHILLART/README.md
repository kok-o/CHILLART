# 🎮 Game Club MVP

2D-игровой зал с автоматами, где игрок может перемещаться по залу, запускать мини-игры, зарабатывать очки и участвовать в рейтинге.

## 🧱 Технологический стек

| Компонент | Технология | Назначение |
|-----------|------------|------------|
| Клиент | Unity 2D | Визуализация зала, мини-игры, UI |
| Авторизация | Firebase Authentication | Вход игрока, UID |
| Сервер | Node.js + Express | API для очков, рейтинга |
| База данных | PostgreSQL | Хранение пользователей, очков |
| Хостинг | Railway / Render / Supabase | Размещение сервера и БД |

## 📁 Структура проекта

```
CHILLART/
├── client/                 # Unity 2D проект
│   ├── Assets/
│   │   ├── Scripts/        # C# скрипты
│   │   ├── Scenes/         # Unity сцены
│   │   ├── Prefabs/        # Префабы
│   │   └── Firebase/       # Firebase конфигурация
│   └── README.md
├── server/                 # Node.js сервер
│   ├── index.js           # Express сервер
│   ├── package.json       # Зависимости
│   └── env.example        # Пример переменных окружения
├── database/              # SQL-скрипты
│   ├── init.sql          # Инициализация БД
│   ├── migrations/       # Миграции
│   └── README.md
├── docs/                 # Документация
│   ├── firebase-setup.md
│   └── unity-scene-setup.md
└── README.md
```

## 🚀 Быстрый старт

### 1. Клонирование репозитория
```bash
git clone <repository-url>
cd CHILLART
```

### 2. Настройка базы данных

#### Установка PostgreSQL
```bash
# Windows (Chocolatey)
choco install postgresql

# macOS (Homebrew)
brew install postgresql

# Ubuntu/Debian
sudo apt-get install postgresql postgresql-contrib
```

#### Создание базы данных
```sql
-- Подключиться к PostgreSQL
psql -U postgres

-- Создать базу данных
CREATE DATABASE game_club;

-- Выйти
\q
```

#### Инициализация схемы
```bash
# Запустить скрипт инициализации
psql -U postgres -d game_club -f database/init.sql
```

### 3. Настройка сервера

#### Установка зависимостей
```bash
cd server
npm install
```

#### Настройка переменных окружения
```bash
# Скопировать пример конфигурации
cp env.example .env

# Отредактировать .env файл
# DB_HOST=localhost
# DB_PORT=5432
# DB_NAME=game_club
# DB_USER=postgres
# DB_PASSWORD=your_password
```

#### Запуск сервера
```bash
# Режим разработки
npm run dev

# Продакшн режим
npm start
```

Сервер будет доступен по адресу: `http://localhost:3000`

### 4. Настройка Unity клиента

#### Требования
- Unity 2022.3 LTS или новее
- 2D Template

#### Настройка Firebase
1. Создать проект в [Firebase Console](https://console.firebase.google.com)
2. Включить Anonymous Authentication
3. Скачать конфигурационные файлы:
   - `google-services.json` (Android)
   - `GoogleService-Info.plist` (iOS)
4. Поместить в `client/Assets/Firebase/`

#### Установка Firebase SDK
1. Открыть Unity Package Manager
2. Добавить пакет из Git URL: `https://github.com/firebase/quickstart-unity.git`

#### Настройка сцены
1. Открыть Unity проект в папке `client/`
2. Создать сцену `LobbyScene`
3. Настроить согласно [Unity Scene Setup Guide](docs/unity-scene-setup.md)

## 🎯 Функциональность MVP

### ✅ Реализовано
- [x] Unity 2D сцена с залом игровых автоматов
- [x] 3 автомата: Puzzle, Memory, Arcade
- [x] Телепортация игрока по клику
- [x] Запуск мини-игр (UI-панель)
- [x] Node.js сервер с Express
- [x] API эндпоинты: `/score`, `/leaderboard`
- [x] PostgreSQL база данных
- [x] Firebase Authentication (анонимный вход)
- [x] Система очков и рейтинга

### 🔄 В разработке
- [ ] Реальные мини-игры
- [ ] Магазин жетонов
- [ ] Ежедневные задания
- [ ] Система наград
- [ ] Мультиплеер

## 📡 API Endpoints

### POST /score
Сохранить очки игрока
```json
{
  "uid": "firebase-uid",
  "points": 120,
  "gameType": "puzzle"
}
```

### GET /leaderboard
Получить ТОП игроков
```json
{
  "success": true,
  "leaderboard": [
    {
      "firebase_uid": "user-123",
      "total_points": 1500,
      "games_played": 5
    }
  ]
}
```

### GET /user/:uid/stats
Получить статистику пользователя

### GET /health
Проверка состояния сервера

## 🧪 Тестирование

### Тест сервера
```bash
# Проверка здоровья
curl http://localhost:3000/health

# Отправка очков
curl -X POST http://localhost:3000/score \
  -H "Content-Type: application/json" \
  -d '{"uid":"test-user","points":100,"gameType":"puzzle"}'

# Получение рейтинга
curl http://localhost:3000/leaderboard
```

### Тест Unity клиента
1. Запустить Unity в Play Mode
2. Проверить движение игрока
3. Протестировать взаимодействие с автоматами
4. Проверить отправку очков на сервер

## 🚀 Развертывание

### Railway
```bash
# Установить Railway CLI
npm install -g @railway/cli

# Войти в аккаунт
railway login

# Инициализировать проект
railway init

# Развернуть
railway up
```

### Render
1. Подключить GitHub репозиторий
2. Настроить переменные окружения
3. Выбрать Node.js шаблон
4. Развернуть

### Supabase
1. Создать проект в Supabase
2. Импортировать SQL схему
3. Настроить переменные окружения
4. Развернуть сервер

## 📚 Документация

- [Firebase Setup Guide](docs/firebase-setup.md)
- [Unity Scene Setup](docs/unity-scene-setup.md)
- [Database Schema](database/README.md)
- [Server API](server/README.md)

## 🐛 Устранение неполадок

### Сервер не запускается
- Проверить подключение к PostgreSQL
- Убедиться в правильности переменных окружения
- Проверить порт 3000

### Unity не подключается к Firebase
- Проверить конфигурационные файлы
- Убедиться в правильности пакета Firebase
- Проверить интернет-соединение

### База данных недоступна
- Проверить статус PostgreSQL
- Убедиться в правильности учетных данных
- Проверить сетевые настройки

## 🤝 Вклад в проект

1. Форкнуть репозиторий
2. Создать ветку для новой функции
3. Внести изменения
4. Создать Pull Request

## 📄 Лицензия

MIT License - см. файл LICENSE

## 👥 Авторы

- Разработчик MVP

## 📞 Поддержка

При возникновении проблем:
1. Проверить документацию
2. Создать Issue в репозитории
3. Обратиться к команде разработки

---

**Статус проекта**: MVP готов к тестированию 🎮
