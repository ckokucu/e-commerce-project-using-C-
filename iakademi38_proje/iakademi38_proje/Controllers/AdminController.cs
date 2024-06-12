using Microsoft.AspNetCore.Mvc;
using iakademi38_proje.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Design;

namespace iakademi38_proje.Controllers
{
    public class AdminController : Controller
    {
        Cls_User u = new Cls_User();
        Cls_Category c = new Cls_Category();
        Cls_Supplier s = new Cls_Supplier();
        Cls_Status st = new Cls_Status();
        Cls_Product p=new Cls_Product();
        
        iakademi38Context context= new iakademi38Context();

        // Giriş ekranının gelmesi get
        //Derfault u get yazmazsan o get anlar
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        #region MyRegion
        //Metod overload
        //Göndere-girişe  tıkladıgında gidecegi yer post
        //Veriler User tablosunda kontrol edilceğinden User tipinde 
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]

        //Bİnd =.cshtml sayfasından gelecek olan property listesinin dısında kabul etmez(email,password,nameSurname)
        //async Task, await asencronize metod yzarken
        //ModelState.IsValid=propertylerdeki bütün zorunlu alanların kontrolü
        public async Task<IActionResult> Login([Bind("Email,Password,NameSurname")] User user)
        {
            if (ModelState.IsValid)
            {
                User? usr = await u.loginControl(user);
                if (usr != null)
                {
                    //HttpContext.Session.SetString("Email","deneme");
                    return RedirectToAction("Index");
                }
            }

            else
            {
                ViewBag.error = "Login ve/veya şifre yanlış";
            }

            return View();
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CategoryIndex()
        {
            //geri dönüş tipi = List<Category>
            //değişken adı = categories
            //metod içinden,eşitliğin sağında gelecek değer(hangi clasta, hangi isimli metod)
            List<Category> categories = await c.CategorySelect();

            return View(categories);


        }

        [HttpGet]
        public IActionResult CategoryCreate()
        {
            CategoryFill();
            return View();
        }

        void CategoryFill()
        {
            List<Category> categories = c.CategorySelectMain();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });
        }

        [HttpPost]
        public IActionResult CategoryCreate(Category category)
        {
            bool answer = Cls_Category.CategoryInsert(category);
            if (answer)
            {
                TempData["Message"] = category.CategoryName + " Eklendi";
            }
            else
            {
                TempData["Message"] = "HATA";
            }
            return RedirectToAction(nameof(CategoryCreate)); //CategoryCreate.cshtml ye [HttpGet] ten gider
                                      //return view(); şuan [HttpPost]ta oldugum icin , CategoryCreate.cshtmlye [HttpPost]tan gider
        }

        [HttpGet]
        public async Task<IActionResult> CategoryEdit(int? id)
        {
            CategoryFill();
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await c.CategoryDetails(id);

            return View(category);
        }

        [HttpPost]
        public IActionResult CategoryEdit(Category category)
        {
            bool answer = Cls_Category.CategoryUpdate(category);
            if (answer)
            {
                TempData["Message"] = "Güncellendi";
                return RedirectToAction("CategoryIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof (CategoryEdit));
            }
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(int? id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c=>c.CategoryID==id);
            ViewBag.categoryname = category?.CategoryName;

            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> CategoryDelete(int? id) //nullsa demek soru işareti
        {
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await context.Categories.FirstOrDefaultAsync(c=>c.CategoryID==id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost,ActionName("CategoryDelete")]
        public async Task<IActionResult> CategoryDeleteConfirmed(int id)
        {
            bool answer = Cls_Category.CategoryDelete(id);
            if (answer)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("CategoryIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof(CategoryDelete));// "CategoryDelete" de yazabilirdin nameof yazmadan
            }
        }

        public async Task<IActionResult> SupplierIndex()
        {
            //geri dönüş tipi = List<Category>
            //değişken adı = categories
            //metod içinden,eşitliğin sağında gelecek değer(hangi clasta, hangi isimli metod)
            List<Supplier> suppliers = await s.SupplierSelect();

            return View(suppliers);


        }


		[HttpGet]
		public IActionResult SupplierCreate()
		{
			
			return View();
		}

        [HttpPost]
        public IActionResult SupplierCreate(Supplier supplier)
        {
            //supplier.PhotoPath.HasFile y da  Exists ile resimler klasöründe var mı
            //yoksa
            //.SaveAs(Server.MapPath("~/resimler/")+supplier.PhotoPath);//resminyükleneceği adres
            bool answer = Cls_Supplier.SupplierInsert(supplier);
            if (answer)
            {
                TempData["Message"] =supplier.BrandName+" Markası Eklendi";
            }
            else
            {
                TempData["Message"] = "HATA "+supplier.BrandName+" Markası Eklenemedi" ;
            }
            return RedirectToAction(nameof(SupplierCreate)); //[HttpGet] metodundann, suppliercreate.cshtml sayfasına gidiyoruz
        }


        public async Task<IActionResult> SupplierEdit(int? id)
        {
            if (id == null||context.Suppliers==null)
            {
                return NotFound();
            }

            var supplier = await s.SupplierDetails(id); 

            return View(supplier);
        }

        [HttpPost]
        public IActionResult SupplierEdit(Supplier supplier)
        {
            if (supplier.PhotoPath == null)
            {
                string? PhotoPath = context.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID).PhotoPath;
                supplier.PhotoPath = PhotoPath;
            }

            bool answer = Cls_Supplier.SupplierUpdate(supplier);
            if (answer)
            {
                TempData["Message"] = "Güncellendi";
                return RedirectToAction("SupplierIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof(SupplierEdit));
            }
        }


        public async Task<IActionResult> SupplierDetails(int? id)
        {
            var supplier = await context.Suppliers.FirstOrDefaultAsync(su => su.SupplierID == id);
            ViewBag.brandName = supplier?.BrandName;

            return View(supplier);
        }



        public async Task<IActionResult> SupplierDelete(int? id) //nullsa demek soru işareti
        {
            if (id == null || context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }


        [HttpPost, ActionName("SupplierDelete")]
        public async Task<IActionResult> SupplierDeleteConfirmed(int id)
        {
            bool answer = Cls_Supplier.SupplierDelete(id);
            if (answer)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("SupplierIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof(SupplierDelete));// "CategoryDelete" de yazabilirdin nameof yazmadan
            }
        }


        public async Task<IActionResult> StatusIndex()
        {

            List<Status> statuses = await st.StatusSelect();

            return View(statuses);//statuses i unutursak  kayıtlar gelmez


        }

		public IActionResult StatusCreate()
		{

			return View();
		}

		[HttpPost]
		public IActionResult StatusCreate(Status status)
		{
			//supplier.PhotoPath.HasFile y da  Exists ile resimler klasöründe var mı
			//yoksa
			//.SaveAs(Server.MapPath("~/resimler/")+supplier.PhotoPath);//resminyükleneceği adres
			bool answer = Cls_Status.StatusInsert(status);
			if (answer)
			{
				TempData["Message"] = status.StatusName + " Statü Eklendi";
			}
			else
			{
				TempData["Message"] = "HATA " + status.StatusName + " Statü Eklenemedi";
			}
			return RedirectToAction(nameof(StatusCreate)); //[HttpGet] metodundann, suppliercreate.cshtml sayfasına gidiyoruz
		}


		public async Task<IActionResult> StatusEdit(int? id)
		{
			if (id == null || context.Statuses == null)
			{
				return NotFound();
			}

			var statuses = await st.StatusDetails(id);

			return View(statuses);
		}

		[HttpPost]
		public IActionResult StatusEdit(Status status)
		{


			bool answer = Cls_Status.StatusUpdate(status);
			if (answer)
			{
				TempData["Message"] = "Güncellendi";
				return RedirectToAction("StatusIndex");
			}
			else
			{
				TempData["Message"] = "HATA";
				return RedirectToAction(nameof(StatusEdit));
			}
		}

		public async Task<IActionResult> StatusDelete(int? id) //nullsa demek soru işareti
		{
			if (id == null || context.Statuses == null)
			{
				return NotFound();
			}

			var status = await context.Statuses.FirstOrDefaultAsync(c => c.StatusID == id);

			if (status == null)
			{
				return NotFound();
			}

			return View(status);
		}

		[HttpPost, ActionName("StatusDelete")]
		public async Task<IActionResult> StatusDeleteConfirmed(int id)
		{
			bool answer = Cls_Status.StatusDelete(id);
			if (answer)
			{
				TempData["Message"] = "Silindi";
				return RedirectToAction("StatusIndex");
			}
			else
			{
				TempData["Message"] = "HATA";
				return RedirectToAction(nameof(StatusIndex));// "StatusDelete" de yazabilirdin nameof yazmadan
			}
		}

        public async Task<IActionResult> ProductIndex()
        {
            List<Product> products = await p.ProductSelect();
            return View(products); 
        }



		public async Task<IActionResult>  ProductCreate()
		{

			List<Category>categories=await c.CategorySelect();
            ViewData["categoryList"]=categories.Select(c=>new SelectListItem { Text=c.CategoryName,Value=c.CategoryID.ToString()});

			List<Supplier> suppliers = await s.SupplierSelect();
			ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });

			List<Status> statuses = await st.StatusSelect();
			ViewData["statusList"] = statuses.Select(st => new SelectListItem { Text = st.StatusName, Value = st.StatusID.ToString() });

            return View();
		}

		
        [HttpPost]
        public IActionResult ProductCreate(Product product)
        {
            bool answer = Cls_Product.ProductInsert(product);
            if (answer == true)
            {
                TempData["Message"] =product.ProductName +" Eklendi";
            }
            else
            {
                TempData["Message"] = "HATA";
            }
            return RedirectToAction(nameof(ProductCreate));
        }





		public async Task<IActionResult> ProductEdit(int? id)
		{
			CategoryFill();
			SupplierFill();
			StatusFill();

			if (id == null || context.Products == null)
			{
				return NotFound();
			}

			var product = await p.ProductDetails(id);

			return View(product);
		}

		[HttpPost]
		public IActionResult ProductEdit(Product product)
		{
			//veritabanından kaydını getirdim
			Product prd = context.Products.FirstOrDefault(s => s.ProductID == product.ProductID);
			//formdan gelmeyen , bazı kolonları null yerine , eski bilgilerini bastım
			product.AddDate = prd.AddDate;
			product.HighLighted = prd.HighLighted;
			product.TopSeller = prd.TopSeller;

			if (product.PhotoPath == null)
			{
				string? PhotoPath = context.Products.FirstOrDefault(s => s.ProductID == product.ProductID).PhotoPath;
				product.PhotoPath = PhotoPath;
			}

			bool answer = Cls_Product.ProductUpdate(product);
			if (answer == true)
			{
				TempData["Message"] = "Güncellendi";
				return RedirectToAction("ProductIndex");
			}
			else
			{
				TempData["Message"] = "HATA";
				return RedirectToAction(nameof(ProductEdit));
			}
		}

        async void SupplierFill()
        {
            List<Supplier> suppliers = await s.SupplierSelect();
            ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });
        }

        async void StatusFill()
        {
            List<Status> statuses = await st.StatusSelect();
            ViewData["StatusList"] = statuses.Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusID.ToString() });
        }


        public async Task<IActionResult> ProductDetails(int? id)
        {
            var product = await context.Products.FirstOrDefaultAsync(c => c.ProductID == id);
            ViewBag.productname = product?.ProductName;

            return View(product);
        }

        public async Task<IActionResult> ProductDelete(int? id)
        {
            if (id == null||context.Products==null)
            {
                return NotFound();
            }

            var product = await context.Products.FirstOrDefaultAsync(c => c.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);   

            //return RedirectToAction("productIndex"); direk silip sayfaya doncek
        }

        [HttpPost, ActionName("ProductDelete")]
        public async Task<IActionResult> ProductDeleteConfirmed(int id)
        {
            bool answer = Cls_Product.ProductDelete(id);
            if (answer)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof(ProductIndex));// "ProductDelete" de yazabilirdin nameof yazmadan
            }
        }


    }
}
