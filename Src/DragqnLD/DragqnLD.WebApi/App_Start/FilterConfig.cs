using System.Web;
using System.Web.Mvc;
using DragqnLD.WebApi.Filters;

namespace DragqnLD.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
