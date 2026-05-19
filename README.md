# LinguaQuest

ASP.NET Core Blazor Server application prepared for deployment on `linguaquest.pp.ua`.

## Required production environment variables

Set the MongoDB connection string on the hosting platform instead of storing credentials in `appsettings.json`:

```bash
MongoDbSettings__ConnectionString="mongodb+srv://USER:PASSWORD@HOST/?appName=LinguaQuest"
MongoDbSettings__DatabaseName="LinguaQuestDb"
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="http://+:8080"
```

## Docker deployment

Build the image:

```bash
docker build -t linguaquest .
```

Run the container:

```bash
docker run -d \
  --name linguaquest \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e MongoDbSettings__ConnectionString="mongodb+srv://USER:PASSWORD@HOST/?appName=LinguaQuest" \
  -e MongoDbSettings__DatabaseName="LinguaQuestDb" \
  linguaquest
```

## Domain setup for linguaquest.pp.ua

In DNS, point the domain to the server or hosting provider:

- `A` record: `linguaquest.pp.ua` -> your server IPv4 address
- `A` record: `www.linguaquest.pp.ua` -> your server IPv4 address

If your host gives you a hostname instead of an IP, use `CNAME` according to that provider's instructions.

## Nginx reverse proxy example

Use this if the application runs on a VPS behind Nginx:

```nginx
server {
    server_name linguaquest.pp.ua www.linguaquest.pp.ua;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

After that, enable HTTPS with Certbot or your hosting provider's SSL settings.

## Local development

```bash
dotnet restore
dotnet run
```

The local development URL is configured in `Properties/launchSettings.json`.
