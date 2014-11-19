using System.Web.Mvc;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// The home page controller
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The home page action
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
