using System.Net.Mime;
using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    [HttpGet("excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExcel(
        [FromQuery] RequestGenerateBillingsReportJson request,
        [FromServices] IGenerateBillingsReportUseCase useCase
    )
    {
        byte[] file = await useCase.Execute(request);

        if (file.Length > 0)
        {
            return File(
                file,
                MediaTypeNames.Application.Octet,
                "report.xlsx"
            );
        }

        return NoContent();
    }
}