using Cmed.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cmed.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ConformityController(
    IConformityService conformityService
)
{
    private readonly IConformityService _conformityservice = conformityService;

    [HttpGet("is-updated")]
    public ActionResult<bool> GetIsUpdated(DateTimeOffset queryDateTimeOffset)
    {
        return _conformityservice.GetIsUpdated(queryDateTimeOffset);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Stream fstream = await _conformityservice.GetLatestFileAsync();
        return new FileStreamResult(fstream, "text/csv")
        {
            FileDownloadName = "conformidade.csv"
        };
    }
}