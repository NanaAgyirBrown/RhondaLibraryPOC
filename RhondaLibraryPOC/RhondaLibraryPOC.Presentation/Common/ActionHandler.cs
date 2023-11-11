using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RhondaLibraryPOC.Presentation.Common;

internal static class ActionHandler
{
    internal static IActionResult HandleActionResult<T>(ErrorOr<T> result, HttpStatusCode successStatusCode = HttpStatusCode.OK, string routeName = null)
    {
        return result.Match<IActionResult>(
            value => value != null
                ? new OkObjectResult(value)
                : new NotFoundObjectResult("Resource not found"),
            error =>
            {
                var firstError = result.ErrorsOrEmptyList.FirstOrDefault();
                var statusCode = int.TryParse(firstError.Code, out var code)
                    ? (HttpStatusCode)code
                    : HttpStatusCode.BadRequest;

                return statusCode == HttpStatusCode.NotFound
                    ? new NotFoundObjectResult(firstError.Description)
                    : new BadRequestObjectResult(error);
            }
        );
    }
}
