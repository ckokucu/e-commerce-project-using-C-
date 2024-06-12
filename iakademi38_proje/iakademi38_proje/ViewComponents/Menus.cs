using iakademi38_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi38_proje.ViewComponents
{
	public class Menus:ViewComponent
	{
		iakademi38Context context=new iakademi38Context();

		public IViewComponentResult Invoke()
		{
			List<Category> categories=context.Categories.ToList();
			return View(categories);	
		}

	}
}
