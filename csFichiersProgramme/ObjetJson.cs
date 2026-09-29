using System.Text.Json;

public abstract class ObjetJson
{
    public virtual string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            
        };

        return JsonSerializer.Serialize(this, GetType(), options);
    }
}