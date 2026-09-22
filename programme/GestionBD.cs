using Npgsql;
using NpgsqlTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ =>
    NpgsqlDataSource.Create(
        "Host=localhost;Database=tp1_julien;Username=robot;Password=yfzvjp9y3s9j37q"
    )
);

var app = builder.Build();

app.MapPost("/api/loueurs", async (
    LoueurRequest request,
    NpgsqlDataSource database) =>
{
    await using var command = database.CreateCommand("""
        INSERT INTO loueur (
            nom,
            prenom,
            salaire_heure,
            id_succursale,
            contact
        )
        VALUES ($1, $2, $3, $4, $5)
        RETURNING id
        """);

    command.Parameters.AddWithValue(request.Nom);
    command.Parameters.AddWithValue(request.Prenom);
    command.Parameters.AddWithValue(request.SalaireHeure);
    command.Parameters.AddWithValue(request.IdSuccursale);

    var contactJson = System.Text.Json.JsonSerializer.Serialize(request.Contact);
    command.Parameters.AddWithValue(
        NpgsqlDbType.Jsonb,
        contactJson
    );

    try
    {
        var id = await command.ExecuteScalarAsync();

        return Results.Created(
            $"/api/loueurs/{id}",
            new { id }
        );
    }
    catch (PostgresException exception)
        when (exception.SqlState == "23505")
    {
        return Results.Conflict(
            "Le courriel ou le téléphone existe déjà."
        );
    }
});

app.Run();

public record LoueurRequest(
    string Nom,
    string Prenom,
    decimal SalaireHeure,
    int IdSuccursale,
    Contact Contact
);

public record Contact(
    string Email,
    string Telephone,
    Adresse Adresse
);

public record Adresse(
    string Rue,
    string Ville,
    string CodePostal,
    string Pays
);