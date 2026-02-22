using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class FallbackController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public FallbackController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        // GET: FallbackController
        public ActionResult Index()
        {
            var file = Path.Combine(_hostEnvironment.WebRootPath, "index.html");
            return PhysicalFile(file, MediaTypeNames.Text.Html);
        }

    }
}
