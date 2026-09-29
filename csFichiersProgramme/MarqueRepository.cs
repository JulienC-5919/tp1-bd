using Npgsql;

public class MarqueRepository(NpgsqlDataSource dataSource)
{
    public async Task<int> AjouterAsync(String marque)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(
            "INSERT INTO marque (nom) VALUES (@nom)",
            connection);

        command.Parameters.AddWithValue("nom", marque);

        return await command.ExecuteNonQueryAsync();
    }
}