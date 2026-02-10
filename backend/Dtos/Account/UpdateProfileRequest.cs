using System.ComponentModel.DataAnnotations;
using backend.Validation;

namespace backend.Dtos;

public sealed record UpdateProfileRequest(
    [param: NotWhiteSpaceIfProvided, StringLength(ValidationConstants.NameMaxLength)] string? Name,
    [param: DateInPastIfProvided] DateTime? DateOfBirth);
