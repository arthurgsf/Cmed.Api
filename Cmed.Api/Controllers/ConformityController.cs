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

    [HttpGet("/is-updated")]
    public ActionResult<bool> GetIsUpdated(DateTimeOffset queryDateTimeOffset)
    {
        return _conformityservice.GetIsUpdated(queryDateTimeOffset);
    }

    [HttpGet]
    public IActionResult Get()
    {
        Stream fstream = _conformityservice.GetLatestFile();
        return new FileStreamResult(fstream, "text/csv")
        {
            FileDownloadName = "conformidade.csv"
        };
    }
}