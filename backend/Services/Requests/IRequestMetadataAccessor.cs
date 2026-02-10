namespace backend.Services.Requests;

public interface IRequestMetadataAccessor
{
    string UserAgent { get; }
    string? IpAddress { get; }
}
