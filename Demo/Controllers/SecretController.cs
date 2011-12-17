using System.Web.Mvc;

namespace Demo.Controllers
{
    [Authorize]
    public class SecretController : Controller
    {
        //
        // GET: /Secret/

        public ActionResult Index()
        {
            return View();
        }

    }
}
