using backend.Validation;

namespace backend.Dtos;

public sealed record ChangePhoneRequest(
    [param: NotWhiteSpace, PhoneE164] string PhoneE164);
