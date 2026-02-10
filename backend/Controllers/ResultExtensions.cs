using backend.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkResult(),
            ResultStatus.BadRequest => new BadRequestResult(),
            ResultStatus.Unauthorized => new UnauthorizedResult(),
            ResultStatus.Forbidden => new ForbidResult(),
            ResultStatus.NotFound => new NotFoundResult(),
            ResultStatus.Conflict => new ConflictResult(),
            ResultStatus.TooManyRequests => new StatusCodeResult(StatusCodes.Status429TooManyRequests),
            ResultStatus.ServiceUnavailable => new StatusCodeResult(StatusCodes.Status503ServiceUnavailable),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(result.Value),
            ResultStatus.BadRequest => new BadRequestResult(),
            ResultStatus.Unauthorized => new UnauthorizedResult(),
            ResultStatus.Forbidden => new ForbidResult(),
            ResultStatus.NotFound => new NotFoundResult(),
            ResultStatus.Conflict => new ConflictResult(),
            ResultStatus.TooManyRequests => new StatusCodeResult(StatusCodes.Status429TooManyRequests),
            ResultStatus.ServiceUnavailable => new StatusCodeResult(StatusCodes.Status503ServiceUnavailable),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }
}
