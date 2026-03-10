using BankMap.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMap.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ISender Mediator => HttpContext.RequestServices.GetRequiredService<ISender>();
        protected ILogger<ApiControllerBase> Logger => HttpContext.RequestServices.GetRequiredService<ILogger<ApiControllerBase>>();

        protected async Task<IActionResult> SendAsync<T>(IRequest<Result<T>> req)
        {
            var ct = HttpContext.RequestAborted;

            var requestedName = req.GetType().Name;
            Logger.LogInformation("Sending request: {RequestName}", requestedName);

            var res = await Mediator.Send(req, ct);

            if (res.IsSuccess)
            {
                Logger.LogInformation("Request {RequestName} handled successfully", requestedName);

                if (res.Value is Unit || typeof(T) == typeof(Unit))
                    return NoContent();

                return Ok(res.Value);
            }

            Logger.LogWarning("Request {RequestName} failed: {Error}", requestedName, res.Error);
            return BadRequest(res.Error);
        }
    }
}
