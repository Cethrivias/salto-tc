# salto-tc

## Docker compose

Use docker compose to start the project and the database

```bash
docker-compose up -d
```

## Manual

You can start the project manually 

```bash
dotnet run --project api
```

It requires a MySql DB.

Don't forget to update `Main` connection string in `Api/appsettings.Development.json`

You can also populate the DB using `MySql/init/users.sql` script