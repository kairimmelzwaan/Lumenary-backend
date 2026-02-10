Lumenary Backend

requirements
- .NET 10.0 sdk
- PostgreSQL

setup
1. configure settings:
    - copy `backend/appsettings.template.json` to `backend/appsettings.json`.
    - update `ConnectionStrings:Default` and `Auth:SessionTokenKey` to a random generated string.
    - update `SeedUser` for an automatic admin in the application.
2. create the database:
    - `dotnet ef database update --project backend --startup-project backend`

run
- `dotnet run --project backend`

the api will start with the defaults in `backend/appsettings.template.json`.

this program uses Swagger for the time being.

you can find it on `http://localhost:port/swagger/index.html`
