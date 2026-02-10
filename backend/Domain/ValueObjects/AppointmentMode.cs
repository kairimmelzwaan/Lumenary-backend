namespace backend.Domain.ValueObjects;

public readonly record struct AppointmentMode
{
    public const string InPerson = "in_person";
    public const string Telehealth = "telehealth";
    public const string Phone = "phone";

    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        InPerson,
        Telehealth,
        Phone
    };

    public string Value { get; }

    private AppointmentMode(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public static bool TryParse(string? input, out AppointmentMode mode)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            mode = default;
            return false;
        }

        var normalized = input.Trim().ToLowerInvariant();
        if (!Allowed.Contains(normalized))
        {
            mode = default;
            return false;
        }

        mode = new AppointmentMode(normalized);
        return true;
    }
}
