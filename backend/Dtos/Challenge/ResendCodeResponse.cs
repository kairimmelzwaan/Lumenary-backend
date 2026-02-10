using System.Text.Json.Serialization;

namespace backend.Dtos;

public sealed record ResendCodeResponse(
    Guid ChallengeId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Code);
