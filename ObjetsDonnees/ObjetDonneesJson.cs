public abstract class ObjetDonneesJson : ObjetDonnees
{
    protected abstract object JsonRepresentation { get; }
    public String ToJson()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        return JsonSerializer.Serialize(
            JsonRepresentation,
            options);
    }
}