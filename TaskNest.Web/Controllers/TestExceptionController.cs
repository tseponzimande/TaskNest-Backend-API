namespace TaskNest.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestExceptionController : ControllerBase
    {
        // just testing
        [HttpGet("throw")]
        public IActionResult Throw()
        {
            throw new System.Exception("Test exception from TestExceptionController.");
        }
    }
}
