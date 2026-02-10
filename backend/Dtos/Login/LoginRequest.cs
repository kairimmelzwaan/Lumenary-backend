using System.ComponentModel.DataAnnotations;
using backend.Validation;

namespace backend.Dtos;

public sealed record LoginRequest(
    [param: NotWhiteSpace, EmailAddress, StringLength(ValidationConstants.EmailMaxLength)] string Email,
    [param: NotWhiteSpace, StringLength(ValidationConstants.PasswordMaxLength)] string Password);
