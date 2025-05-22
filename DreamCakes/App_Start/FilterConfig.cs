using System.Web;
using System.Web.Mvc;
using DreamCakes.Filters;
using DreamCakes.Utilities;

namespace DreamCakes
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionAuthorizeUtility());
            
        }
    }
}
