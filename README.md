PasteBinLite

Pastebin-Lite is a small, simple web application inspired by Pastebin. It allows users to create text
pastes and share them using a unique link. Each paste can optionally expire after a certain time or
after a limited number of views.

Live Application
Frontend: https://pruthvirajpallykonda.github.io/PasteBin/
Backend API: https://pastebin-production-2639.up.railway.app

What This App Does
• Lets users create a paste with plain text
• Generates a shareable URL for every paste
• Allows optional expiration using time (TTL)
• Allows optional expiration using view count
• Automatically removes access once limits are reached

Tech Stack
Frontend: React + Vite (hosted on GitHub Pages)
Backend: ASP.NET Core Web API (.NET 8)
Database: PostgreSQL (Neon)

Persistence Layer
PostgreSQL is used to store all pastes and metadata. This ensures data is not lost between
requests and works correctly in a serverless environment. The database stores paste content,
expiry time, and remaining views.

API Overview
Create Paste: POST /api/pastes
Fetch Paste: GET /api/pastes/{id}
Health Check: GET /api/healthz

Deterministic Time Support
For automated testing, the API supports a test mode. When TEST_MODE=1 is enabled, the current
time is read from the request header: x-test-now-ms. This allows consistent and repeatable test
results.

Design Decisions
• Chose database-backed storage instead of in-memory storage to ensure reliability across restarts and deployments.
• Handles all invalid or expired pastes with HTTP 404
• Keeps frontend and backend completely separate
• Designed to pass automated test cases reliably
Purpose
This project was created as part of a take-home assignment to demonstrate backend design, API
handling, frontend integration, and deployment skills

AI tools (ChatGPT) were used selectively during development for architectural review,
edge-case identification, and validation of best practices. All core implementation,
integration, debugging, and deployment decisions were independently designed and
executed by me.

