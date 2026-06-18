using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Authentication required by default; opt out with [AllowAnonymous] (e.g. AuthController).
    public abstract class BaseController : ControllerBase
    {
        private static readonly string[] SupportedLanguages = ["en-US", "my-MM"];

        protected IActionResult Execute<T>(Result<T> result)
        {
            return result.GetEnumRespType() switch
            {
                EnumRespType.Success => Ok(result),
                EnumRespType.Error => BadRequest(result),
                EnumRespType.ValidationError => BadRequest(result),
                EnumRespType.InvalidData => BadRequest(result),
                EnumRespType.DuplicateRecord => Conflict(result),
                EnumRespType.NotFound => NotFound(result),
                EnumRespType.BadRequest => BadRequest(result),
                EnumRespType.Warning => BadRequest(result),
                EnumRespType.SystemError => StatusCode(StatusCodes.Status500InternalServerError, result),
                EnumRespType.None => throw new InvalidOperationException(
                    "Result.Type is None. A service returned a Result without setting its Type."),
                _ => throw new InvalidOperationException("Unhandled EnumRespType in BaseController.Execute.")
            };
        }
    }
}
