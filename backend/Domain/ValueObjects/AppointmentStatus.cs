namespace backend.Domain.ValueObjects;

public enum AppointmentStatusParseMode
{
    DefaultToScheduled,
    RequireValue
}

public readonly record struct AppointmentStatus
{
    public const string Scheduled = "scheduled";
    public const string Confirmed = "confirmed";
    public const string Completed = "completed";
    public const string Canceled = "canceled";
    public const string NoShow = "no_show";
    public const string Rescheduled = "rescheduled";

    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Scheduled,
        Confirmed,
        Completed,
        Canceled,
        NoShow,
        Rescheduled
    };

    public string Value { get; }

    private AppointmentStatus(string value)
    {
        Value = value;
    }

    public bool IsCanceled => string.Equals(Value, Canceled, StringComparison.Ordinal);

    public override string ToString() => Value;

    public static bool TryParse(string? input, AppointmentStatusParseMode mode, out AppointmentStatus status)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            if (mode == AppointmentStatusParseMode.DefaultToScheduled)
            {
                status = new AppointmentStatus(Scheduled);
                return true;
            }

            status = default;
            return false;
        }

        var normalized = input.Trim().ToLowerInvariant();
        if (!Allowed.Contains(normalized))
        {
            status = default;
            return false;
        }

        status = new AppointmentStatus(normalized);
        return true;
    }
}
