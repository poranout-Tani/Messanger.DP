# 💬 Messenger — Full-Stack Real-Time Messaging App

[![Flutter](https://img.shields.io/badge/Flutter-3.x-02569B?logo=flutter&logoColor=white)](https://flutter.dev)
[![Dart](https://img.shields.io/badge/Dart-3.x-0175C2?logo=dart&logoColor=white)](https://dart.dev)
[![.NET](https://img.shields.io/badge/.NET-8%2F9%2F10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12%2F13-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Полнофункциональный кроссплатформенный мессенджер с клиент-серверной архитектурой.
Клиентская часть разработана на **Flutter/Dart**, серверная часть — на **C# / ASP.NET Core** с использованием **Entity Framework Core** и **PostgreSQL**.

---

## 🚀 Основной функционал

* 🔐 **Аутентификация и безопасность:**
  * Регистрация и авторизация пользователей.
  * Безопасная аутентификация через **JWT (JSON Web Tokens)**.
  * Хэширование паролей на стороне бэкенда.
  * Хранение токенов на клиенте с помощью `flutter_secure_storage`.
* 💬 **Чаты и обмен сообщениями:**
  * Личные диалоги 1-на-1 и групповые чаты (`group_create_screen`).
  * Список чатов с отображением последних сообщений и времени.
  * Интерактивный экран переписки на базе `flutter_chat_ui`.
* 👥 **Контакты:**
  * Управление списком контактов, добавление и поиск пользователей.
* 🖼️ **Профиль и медиа:**
  * Личный профиль с поддержкой `Bio` и даты рождения.
  * Загрузка аватарок на сервер с предварительным редактированием через `pro_image_editor`.
* ⚙️ **Настройки:**
  * Экран настроек приложения и параметров профиля.

---

## 🛠 Технологический стек

### Frontend (Клиент)
* **Фреймворк:** [Flutter](https://flutter.dev) (Dart 3.x).
* **Сетевой слой:** [Dio](https://pub.dev/packages/dio) — HTTP-клиент с интерцепторами для передачи JWT-токенов.
* **Чат UI:** `flutter_chat_ui`, `flutter_chat_types`, `flutter_chat_core`.
* **Безопасное хранилище:** `flutter_secure_storage`.
* **Работа с медиа:** `file_picker`, `pro_image_editor`.
* **Десктоп/Окна:** `window_manager`.

### Backend (Сервер)
* **Платформа:** .NET (ASP.NET Core Web API).
* **Архитектурный подход:** Clean / Onion Architecture:
  * `Core` — доменные сущности (`Models`: `Users`, `Chats`, `Messages`, `Contacts`, `ChatMembers`) и контракты данных (`Contracts` / DTO).
  * `Applications` — слой бизнес-логики: сервисы (`ChatMembersServices`, `ChatsService`, `MessagesServices`, `UsersService`) и интерфейсы репозиториев (`Interface/Repositories`: `IUsersRepository`, `IChatsRepository` и др.), реализующие принцип инверсии зависимостей (DIP).
  * `DataAccess` — слой данных: реализация репозиториев (`Repositories`), контекст `MessangerDbContexts`, Fluent API конфигурации сущностей и миграции EF Core.
  * `Infrastructure` (`ClassLibrary1`) — безопасность и токены: генерация и валидация JWT (`JWTProvider`), хэширование паролей (`PasswordHasher`).
  * `Messanger.API` — presentation layer: REST API контроллеры и Minimal Endpoints (`/Users`, `/Chats`, `/Messages`, `/Contacts`), Swagger/OpenAPI, раздача статических файлов (аватары).
* **База данных:** [PostgreSQL](https://www.postgresql.org/) через `Npgsql.EntityFrameworkCore.PostgreSQL`.

---

## 📂 Структура репозитория

```text
├── Messanger.API/                  # Backend на C# / ASP.NET Core
│   ├── Applications/               # Бизнес-логика: Services и Interfaces репозиториев (DIP)
│   ├── ClassLibrary1/              # Инфраструктурный слой (JWT, PasswordHasher)
│   ├── Core/                       # Доменные модели (Models) и контракты (DTOs)
│   ├── DataAccess/                 # Реализация репозиториев, EF Core, миграции
│   ├── Messanger.API/              # Web API проект (Контроллеры, Endpoints, Program.cs)
│   └── Messanger.API.slnx          # Решение Visual Studio
│
├── Messanger.UI/                   # Frontend на Flutter
│   └── flutter_messanger_ui/
│       ├── lib/
│       │   ├── Data/               # ApiClient, TokenStorage, AppConfig
│       │   ├── Screen/             # Экраны (Auth, ChatList, Chat, Profile и др.)
│       │   ├── Utils/              # Вспомогательные утилиты
│       │   └── main.dart           # Точка входа в приложение
│       └── pubspec.yaml            # Зависимости Flutter
│
├── .gitignore                      # Исключение временных файлов сборки и кэша
└── README.md                       # Документация проекта
```

---

## 🏁 Быстрый старт

### 1. Требования
* [.NET 8+ SDK](https://dotnet.microsoft.com/download)
* [Flutter SDK (>= 3.4.0)](https://docs.flutter.dev/get-started/install)
* [PostgreSQL](https://www.postgresql.org/)

---

### 2. Запуск бэкенда (API)

1. Перейдите в каталог API:
   ```bash
   cd "Messanger.API/Messanger.API"
   ```
2. Настройте строку подключения к PostgreSQL в `appsettings.Development.json` (при необходимости):
   ```json
   "ConnectionStrings": {
     "MessangerDbContexts": "Host=localhost;Port=5432;Database=MessangerDb;Username=postgres;Password=YOUR_PASSWORD;"
   }
   ```
3. Примените миграции базы данных:
   ```bash
   dotnet ef database update --project ../DataAccess
   ```
4. Запустите сервер:
   ```bash
   dotnet run
   ```
   Сервер запустится по адресу `http://localhost:5000` (или `https://localhost:7000`), а Swagger UI будет доступен по адресу `/swagger`.

---

### 3. Запуск фронтенда (Flutter)

1. Перейдите в каталог Flutter приложения:
   ```bash
   cd "Messanger.UI/flutter_messanger_ui"
   ```
2. Установите зависимости:
   ```bash
   flutter pub get
   ```
3. При необходимости укажите базовый URL сервера в `lib/Data/app_config.dart`.
4. Запустите приложение:
   ```bash
   flutter run
   ```

---

## 👨‍💻 Автор
* GitHub: [@poranout-Tani](https://github.com/poranout-Tani)
* Репозиторий: [Messanger.DP](https://github.com/poranout-Tani/Messanger.DP)
* Проект разработан для демонстрации навыков full-stack разработки на Flutter и C#/.NET.
