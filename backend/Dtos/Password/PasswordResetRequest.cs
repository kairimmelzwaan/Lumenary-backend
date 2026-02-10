using System.ComponentModel.DataAnnotations;
using backend.Validation;

namespace backend.Dtos;

public sealed record PasswordResetRequest(
    [param: NotWhiteSpace, EmailAddress, StringLength(ValidationConstants.EmailMaxLength)] string Email);
