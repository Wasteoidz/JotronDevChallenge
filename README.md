# Jotron Certificate App

A minimal .NET 10 certificate upload and download app.

## Overview

- Upload certificate files via `POST /certificates`
- Download certificate files via `GET /certificates/{id}/download`
- Uses SQLite locally and seeds mock certificates in development
- Stores uploaded files outside `wwwroot` for safer static hosting

## Run locally

```bash
git clone https://github.com/Wasteoidz/JotronDevChallenge.git
cd JotronDevChallenge
dotnet restore
dotnet run
```

By default, `dotnet run` uses `Development` mode, which seeds the local database.

## Notes

- Uploaded files are stored in `uploads/` inside the app root.
- The API returns safe DTOs and does not expose the internal `FilePath`.
- File uploads are validated for permitted extensions and MIME types.

## Endpoints

- `GET /certificates`
- `GET /certificates/{id}`
- `POST /certificates` (form upload)
- `GET /certificates/{id}/download`
- `DELETE /certificates/{id}`
