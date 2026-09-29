using Npgsql;

public class ClientRepository(NpgsqlDataSource dataSource)
{
    public async Task<int> AjouterAsync(Client client)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(
            "INSERT INTO client (nom, prenom, contact) VALUES (@nom, @prenom, @contact)",
            connection);

        // Ajouter les paramètres, dont contact typé jsonb, puis exécuter.
        command.Parameters.AddWithValue("nom", client.Nom);
        command.Parameters.AddWithValue("prenom", client.Prenom);
        command.Parameters.AddWithValue("contact", NpgsqlTypes.NpgsqlDbType.Jsonb, client.Contact.ToJson());

        return await command.ExecuteNonQueryAsync();
    }
}