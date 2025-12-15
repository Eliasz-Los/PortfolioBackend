using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/")]
public class DoctorController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}