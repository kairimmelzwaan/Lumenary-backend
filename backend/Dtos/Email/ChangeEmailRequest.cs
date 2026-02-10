using System.ComponentModel.DataAnnotations;
using backend.Validation;

namespace backend.Dtos;

public sealed record ChangeEmailRequest(
    [param: NotWhiteSpace, EmailAddress, StringLength(ValidationConstants.EmailMaxLength)] string Email);
