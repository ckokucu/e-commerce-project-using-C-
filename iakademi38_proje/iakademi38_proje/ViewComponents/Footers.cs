using iakademi38_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi38_proje.ViewComponents
{
    public class Footers:ViewComponent
    {
        iakademi38Context context= new iakademi38Context();

        public IViewComponentResult Invoke()
        {
            List<Supplier> suppliers = context.Suppliers.ToList();
            return View(suppliers);
        }
    }
}
