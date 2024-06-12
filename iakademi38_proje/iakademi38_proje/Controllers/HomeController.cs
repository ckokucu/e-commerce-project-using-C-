using Microsoft.AspNetCore.Mvc;
using iakademi38_proje.Models;
using PagedList.Core;
using Microsoft.CodeAnalysis.Differencing;
using NuGet.Common;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net;

namespace iakademi38_proje.Controllers
{
    public class HomeController : Controller
    {

        MainPageModel mpm = new MainPageModel();
        Cls_Product cls_Product = new Cls_Product();
        iakademi38Context context = new iakademi38Context();
        Cls_Order cls_Order = new Cls_Order();
        Cls_User cls_User = new Cls_User();


        int mainpageCount = 0;
        public HomeController()
        {
            this.mainpageCount = context.Settings.FirstOrDefault(s => s.SettingID == 1).MainpageCount;
        }

        public  IActionResult Index()
        {
            mpm.SliderProducts = cls_Product.ProductSelect("Slider", mainpageCount,"Ana",0);
            mpm.Productofday = cls_Product.ProductDetails("Productofday");
            mpm.NewProducts = cls_Product.ProductSelect("New", mainpageCount, "Ana", 0);
            mpm.SpecialProducts = cls_Product.ProductSelect("Special", mainpageCount, "Ana", 0);
            mpm.DiscountedProducts = cls_Product.ProductSelect("Discounted", mainpageCount, "Ana", 0);
            mpm.HighlightedProducts = cls_Product.ProductSelect("HighLighted", mainpageCount, "Ana", 0);
            mpm.TopsellerProducts = cls_Product.ProductSelect("Topseller", mainpageCount, "Ana", 0);
            mpm.StarProducts = cls_Product.ProductSelect("Star", mainpageCount, "Ana", 0);
            mpm.FeaturedProducts = cls_Product.ProductSelect("Featured", mainpageCount, "Ana", 0);
            mpm.NotableProducts = cls_Product.ProductSelect("Notable", mainpageCount, "Ana", 0);
            return View(mpm);
        }


		public IActionResult NewProducts()
		{
			mpm.NewProducts = cls_Product.ProductSelect("New", mainpageCount,"New",0);//yeni
			return View(mpm);
		}


        public PartialViewResult _PartialNewProducts(string pageno)
        {
            int pagenumber=Convert.ToInt32(pageno);
            mpm.NewProducts = cls_Product.ProductSelect("New", mainpageCount, "New", pagenumber);
            return PartialView(mpm);
        }


        public IActionResult SpecialProducts()
        {
            mpm.SpecialProducts = cls_Product.ProductSelect("Special", mainpageCount, "Special", 0);//yeni
            return View(mpm);
        }


        public PartialViewResult _PartialSpecialProducts(string pageno)
        {
            int pagenumber = Convert.ToInt32(pageno);
            mpm.SpecialProducts = cls_Product.ProductSelect("Special", mainpageCount, "Special", pagenumber);
            return PartialView(mpm);
        }



        public IActionResult DiscountedProducts()
        {
            mpm.DiscountedProducts = cls_Product.ProductSelect("Discounted", mainpageCount, "Discounted", 0);//yeni
            return View(mpm);
        }


        public PartialViewResult _PartialDiscountedProducts(string pageno)
        {
            int pagenumber = Convert.ToInt32(pageno);
            mpm.DiscountedProducts = cls_Product.ProductSelect("Discounted", mainpageCount, "Discounted", pagenumber);
            return PartialView(mpm);
        }


        public IActionResult HighlightedProducts()
        {
            mpm.HighlightedProducts = cls_Product.ProductSelect("HighLighted", mainpageCount, "HighLighted", 0);//yeni
            return View(mpm);
        }


        public PartialViewResult _PartialHighlightedProducts(string pageno)
        {
            int pagenumber = Convert.ToInt32(pageno);
            mpm.HighlightedProducts = cls_Product.ProductSelect("HighLighted", mainpageCount, "HighLighted", pagenumber);
            return PartialView(mpm);
        }


        public IActionResult TopsellerProducts(int page = 1, int pageSize = 4)
        {
            //pagedlistcore ınstall edilip usinge eklenddi
            PagedList<Product> model = new PagedList<Product>(context.Products.OrderByDescending(p => p.TopSeller), page, pageSize);

            return View("TopsellerProducts", model); // return view(model) de yazabilirdik
        }






    




        //nıget-Microsoft.AspNetCore.Http
        //Sepete ekle tıklanınca buraya gelecek
        //  /Home/CartProcess/@item.ProductID
        public IActionResult CartProcess(int id)
        {
			Cls_Product.Highlighted_Increase(id);
            cls_Order.ProductID = id;
            cls_Order.Quantity = 1;

            var cookieOptions=new CookieOptions();
            //read
            //10=1&20=1&30=1
            var cookie = Request.Cookies["sepetim"];//sepetim isminde çerezim var mı diye okudu
            if (cookie==null)
            {
                //sepetim isminde çerez yok
                cookieOptions=new CookieOptions();//yarat dedik olmadıgından
                cookieOptions.Expires = DateTime.Now.AddDays(1);//1 günlük çerez
                cookieOptions.Path = "/";
                cls_Order.MyCart = "";
                cls_Order.AddToMyCart(id.ToString());
                Response.Cookies.Append("sepetim", cls_Order.MyCart, cookieOptions);//tarayıcıya gönderdik 
                //HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi.");
                TempData["Message"] = "Ürün Sepetinize Eklendi.";

            }

            else
            {
                //sepet doluysa
                cls_Order.MyCart = cookie;//tarayıadaki sepet bilgisini prop a gönderdim
                if (cls_Order.AddToMyCart(id.ToString())==false)
                {
					//aynı ürün sepette yok,ekleyeceğim
					HttpContext.Response.Cookies.Append("sepetim", cls_Order.MyCart, cookieOptions);//tarayıcıya gönderdik 
					cookieOptions.Expires = DateTime.Now.AddDays(1);
					TempData["Message"] = "Ürün Sepetinize Eklendi.";
				}
                else
                {
					//bu ürün zaten eklenmiş
					TempData["Message"] = "Ürün Sepetinizde Zaten Var.";
				}


            }




            string url = Request.Headers["Referer"].ToString(); //http:lolcallhost:/Home/Firesaturunleri/ ---Hangi sayfadaysam sepete ekle yapınca o sayfada kalsın
            return Redirect(url);
        }

        public async Task< IActionResult> Details(int id)
        {
            Cls_Product.Highlighted_Increase(id);

            //efcore
            //mpm.ProductDetails = context.Products.FirstOrDefault(p => p.ProductID == id);
            //mpm.ProductDetails = await cls_Product.ProductDetails(id);

            //linq
            mpm.ProductDetails = (from p in context.Products where p.ProductID == id select p).FirstOrDefault();
			
            //linq
            mpm.CategoryName=(from p in context.Products join c in context.Categories on p.CategoryID equals c.CategoryID where p.ProductID==id select c.CategoryName).FirstOrDefault();

            //linq
            mpm.BrandName=(from p in context.Products join s in context.Suppliers on p.SupplierID equals s.SupplierID where p.ProductID==id select s.BrandName).FirstOrDefault();


            //Related Products ---> Buna bakanlar   product id si aynı olanı getirmicek yoksa aynısnı buna bakanlarda da basar
            mpm.RelatedProducts=context.Products.Where(p=>p.Related==mpm.ProductDetails!.Related&&p.ProductID!=id).ToList();

            return View(mpm);
        }


        //sag üst koseden sepet sayfama git ikonu tıklanınca ve aynı sayfada ürünü sil butonu tıklanınca 
		public IActionResult Cart()
		{
            List<Cls_Order> sepet; // class dönüyosa propertyler ile veri getireceğim demek oluyo
            //string? scid=HttpContext.Request.Query["scid"]; //ürün silerken sil butonu ile prodcutid gönderecegim

            if (HttpContext.Request.Query["scid"].ToString()!="")
            {
                //sil butonu ile geldim
                string? scid = HttpContext.Request.Query["scid"];
                cls_Order.MyCart = Request.Cookies["sepetim"]; // tarayıcıdan al , property e yaz
                cls_Order.DeleteFromMyCart(scid);//property den sildim
                var cookieOptions = new CookieOptions();
                //tarayıcıya silinmiş halini gönderdim
                Response.Cookies.Append("sepetim", cls_Order.MyCart, cookieOptions);
				cookieOptions.Expires = DateTime.Now.AddDays(1);
				TempData["Message"] = "Ürün Sepetten Silindi.";
                //sepet içindeki son haline cart.cshtml sayfasına da göndereceğim
                sepet = cls_Order.SelectMyCart();
                ViewBag.Sepetim=sepet;
                ViewBag.sepet_tablo_detay=sepet;

			}
            else
            {
                //sag ust koseden sepet sayfasına git ile geldim
                var cookie = Request.Cookies["sepetim"];
                if (cookie==null)
                {
                    cls_Order.MyCart = "";
                    sepet=cls_Order.SelectMyCart();
                    ViewBag.Sepetim = sepet;
                    ViewBag.sepet_tablo_detay = sepet;

                }

                else 
                {
                    var cookieOptions=new CookieOptions();
                    cls_Order.MyCart = Request.Cookies["sepetim"];
                    sepet= cls_Order.SelectMyCart();
                    ViewBag.Sepetim = sepet;
                    ViewBag.sepet_tablo_detay = sepet;

                }
            }



			return View();
		}


        public IActionResult Order()
        {
            if (HttpContext.Session.GetString("Email")!=null)
            {
                //kullanıcı Login.cshtml den giriş yapıp , Session alıp gelmiştir
                User? usr = Cls_User.SelectMemberInfo(HttpContext.Session.GetString("Email"));
                return View(usr);
            }
            else
            {
                //kullanıcı Login.cshtml ye gitmemiş , session alıp gelmemiş
                return RedirectToAction("Login");
            }
        }



        [HttpPost]
        public IActionResult Order(IFormCollection frm)
        {
            string txt_individual = Request.Form["txt_individual"];
            string txt_corporate = Request.Form["txt_corporate"];

            if (txt_individual != null)
            {
                //bireysel fatura
                //digital planet
                //WebServiceController.tckimlik_vergi_no = txt_individual;
                cls_Order.tckimlik_vergi_no = txt_individual;
                cls_Order.EfaturaCreate();
            }
            else
            {
                //kurumsal fatura
               // WebServiceController.tckimlik_vergi_no = txt_corporate;
                cls_Order.tckimlik_vergi_no = txt_corporate;
                cls_Order.EfaturaCreate();
            }

            string kredikartno = Request.Form["kredikartno"]; //formun içindeki textbo alanının degerini IFormCollection kullanmadan yakalamak
            string kredikartay = frm["kredikartay"];//IFormCollection ile yakalamak
            string kredikartyil = frm["kredikartyil"];
            string kredikartcvs = frm["kredikartcvs"];

            return RedirectToAction("backref");// gerçek hayatta bu kaldırılıp aşağıdakiler acılcak

            //buradan sonraki kodlar , payu , iyzico

            //payu dan gelen örnek kodlar

            /*  AŞAGIDAKİ KODLAR GERÇEK HAYATTA AÇILALAK
            NameValueCollection data = new NameValueCollection();
            string url = "https://www.sedattefci.com/backref";
 
            data.Add("BACK_REF", url);
            data.Add("CC_CVV", kredikartcvs);
            data.Add("CC_NUMBER", kredikartno);
            data.Add("EXP_MONTH", kredikartay);
            data.Add("EXP_YEAR", "20" + kredikartyil);
 
            var deger = "";
 
            foreach (var item in data)
            {
                var value = item as string;
                var byteCount = Encoding.UTF8.GetByteCount(data.Get(value));
                deger += byteCount + data.Get(value);
            }
 
            var signatureKey = "size verilen SECRET_KEY buraya yazılacak";
 
            var hash = HashWithSignature(deger, signatureKey);
 
            data.Add("ORDER_HASH", hash);
 
            var x = POSTFormPAYU("https://secure.payu.com.tr/order/....", data);
 
            //sanal kart
            if (x.Contains("<STATUS>SUCCESS</STATUS>") && x.Contains("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
            {
                //sanal kart (debit kart) ile alış veriş yaptı , bankadan onay aldı
            }
            else
            {
                //gerçek kart ile alış veriş yaptı , bankadan onay aldı
            }
            */
        }


        public IActionResult backref()

        {

            ConfirmOrder();

            return RedirectToAction("ConfirmPage");

        }

        public static string OrderGroupGUID = "";

        public IActionResult ConfirmOrder()

        {

            //sipariş tablosuna kaydet

            //sepetim cookie sinden sepeti temizleyecegiz

            //e-fatura olustur metodunu cagır

            var cookieOptions = new CookieOptions();

            var cookie = Request.Cookies["sepetim"];

            if (cookie != null)

            {

                cls_Order.MyCart = cookie;

                OrderGroupGUID = cls_Order.OrderCreate(HttpContext.Session.GetString("Email").ToString());

                cookieOptions.Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Delete("sepetim"); //tarayıcıdan sepeti sil

                //cls_User.Send_Sms(OrderGroupGUID); -------------- suan o uyelikler olmadıgından kapattık

                //cls_User.Send_Email(OrderGroupGUID);

            }

            return RedirectToAction("ConfirmPage");

        }





        public IActionResult ConfirmPage()
        {
            ViewBag.OrderGroupGUID = OrderGroupGUID;
            return View();
        }

        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                List<vw_MyOrders> orders = cls_Order.SelectMyOrders(HttpContext.Session.GetString("Email").ToString());
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }




        //Login
        //Session ile oturum açacagım

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            string answer = Cls_User.MemberControl(user);

            if (answer == "error")
            {
                HttpContext.Session.SetString("Mesaj", "Email/Şifre yanlış girildi");
                TempData["Message"] = "Email/Şifre yanlış girildi";
                return View(); //login sayfasında tekrar denesin
            }
            else if (answer == "admin")
            {
                HttpContext.Session.SetString("Email", answer);
                HttpContext.Session.SetString("Admin", answer);
                return RedirectToAction("Login", "Admin");
                //giriş yaptı dogruysa ve adminse
            }
            else
            {
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index");
                // giriş yapan kullanıcı admin degilse normal kullanıcı indeksine
            }
        }

        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Register(User user)
        {
            if (Cls_User.loginEmailControl(user) == false)
            {
                bool answer = Cls_User.AddUser(user);

                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }
                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }
            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
            
        }

        public IActionResult CategoryPage(int id)
        {
            List<Product> products = cls_Product.ProductSelectWithCategoryID(id);
            return View(products);
        }

        public IActionResult SupplierPage(int id)
        {
            List<Product> products = cls_Product.ProductSelectWithSupplierID(id);
            return View(products);
        }

        public IActionResult DetailSearch()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Suppliers=context.Suppliers.ToList();
            return View();
        }

        public IActionResult DProducts(int CategoryID, string[] SupplierID, string price,string IsInStock)
        {
            int count = 0;
            string suppliervalue = "";//1,3,4 seçtik sqlde

            for (int i = 0; i <SupplierID.Length; i++)
            {
                if (count==0)
                {
                    suppliervalue = "SupplierID=" + SupplierID[i];
                    count++; // or yok sadece 1 tane marka seçilmiş
                }
                else
                {
                    // or var birden fazla marka seçilmiş
                    //suppliervalue=suppliervalue+"or SupplierID="+SupplierID[i] alttaki bu demek yazılımda
                    suppliervalue += " or SupplierID=" + SupplierID[i];

                }
            }

            price= price.Replace(" ","");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0];
            string endprice = PriceArray[1];

            string sign = ">";
            if (IsInStock=="0")
            {
                sign = ">=";
            }


            string query = "select * from products where CategoryID = " + CategoryID + " and (" + suppliervalue + ") and (UnitPrice > " + startprice + " and UnitPrice < " + endprice + ") and Stock " + sign + " 0 order by ProductName";

            ViewBag.Products = cls_Product.SelectProductsByDetails(query);

            return View();
        }

        public PartialViewResult gettingProducts(string id)
        {
            id = id.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            List<Sp_arama> ulist = Cls_Product.gettingSearchProducts(id);
            string json=JsonConvert.SerializeObject(ulist);
            var response = JsonConvert.DeserializeObject<List<Search>>(json);


            return PartialView(response);
        }

        

    }
}
