using System.Web;
using System.Web.Mvc;

namespace wa_asp_mvc_file_operation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
