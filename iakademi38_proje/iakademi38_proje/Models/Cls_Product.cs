using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Debugger.Contracts.EditAndContinue;
using System.Collections.Generic;
using XAct;

namespace iakademi38_proje.Models
{
    public class Cls_Product
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string? PhotoPath { get; set; }

        iakademi38Context context=new iakademi38Context();
		int subpageCount = 0;

        //async , Task<geri dönüş tipi> , await , metotasync olmalı
        public async Task<List<Product>> ProductSelect()
        {

            List<Product> products = await context.Products.ToListAsync();
            return products;
        }

		//metod overload = aynı isimli metodu farklı parametrelerle yazmak
		public List<Product> ProductSelect(string mainPageName,int mainpageCount,string subPageName,int pagenumber)
		{
			subpageCount = context.Settings.FirstOrDefault(s => s.SettingID == 1).SubpageCount;

			List<Product> products;


			if (mainPageName == "Slider")
			{
				products = context.Products.Where(p => p.StatusID == 1 && p.Active == true).Take(mainpageCount).ToList();//take 8 sonradan dinamik yapılacak
			}






			else if (mainPageName == "New")
			{

				if (subPageName == "Ana")
				{
					//Home/Index
					//select top 8 * from products where Active = 1 order by AppDate desc ---> ado.net
					products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Take(mainpageCount).ToList();// --->EFcore
				}
				else//alt sayfa 
				{
					if (pagenumber == 0)
					{
						//altsayfa ilk tıklanış
						products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Take(subpageCount).ToList();
					}
					else
					{
						//AJAX= daha fazla ürün getir     //skip--> zıpla fetch e karşılık geliyo
						products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Skip(pagenumber * subpageCount).Take(subpageCount).ToList();
					}
				}
			}







			else if (mainPageName == "Special")
			{

				if (subPageName == "Ana")
				{
                  
                    products = context.Products.Where(p => p.StatusID == 2 && p.Active == true).Take(mainpageCount).ToList();
                }
				else//alt sayfa 
				{
					if (pagenumber == 0)
					{
                        //altsayfa ilk tıklanış
                        products = context.Products.Where(p => p.StatusID == 2 && p.Active == true).Take(subpageCount).ToList();
					}
					else
					{
                        //AJAX= daha fazla ürün getir     //skip--> zıpla fetch e karşılık geliyo
                        products = context.Products.Where(p => p.StatusID == 2 && p.Active == true).Skip(pagenumber * subpageCount).Take(subpageCount).ToList();

					}
				}
			}







            else if (mainPageName == "Discounted")
            {

                if (subPageName == "Ana")
                {
                    //Home/Index
                    //select top 8 * from products where Active = 1 order by AppDate desc ---> ado.net
                    products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Take(mainpageCount).ToList();// --->EFcore
                }
                else//alt sayfa 
                {
                    if (pagenumber == 0)
                    {
                        //altsayfa ilk tıklanış
                        products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Take(subpageCount).ToList();
                    }
                    else
                    {
                        //AJAX= daha fazla ürün getir     //skip--> zıpla fetch e karşılık geliyo
                        products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Skip(pagenumber * subpageCount).Take(subpageCount).ToList();
                    }
                }
            }










            else if (mainPageName == "HighLighted")
			{

				products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Take(mainpageCount).ToList();
			}

            else if (mainPageName == "HighLighted")
            {

                if (subPageName == "Ana")
                {
                    products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Take(mainpageCount).ToList();
                }
                else//alt sayfa 
                {
                    if (pagenumber == 0)
                    {
                        products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Take(subpageCount).ToList();
                    }
                    else
                    {
                        //AJAX= daha fazla ürün getir     //skip--> zıpla fetch e karşılık geliyo
                        products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Skip(pagenumber * subpageCount).Take(subpageCount).ToList();
                    }
                }
            }









            else if (mainPageName == "Topseller")
			{

				products = context.Products.Where(p => p.Active == true).OrderByDescending(p => p.TopSeller).Take(mainpageCount).ToList();
			}







			else if (mainPageName == "Star")
			{

				products = context.Products.Where(p => p.StatusID == 3 && p.Active == true).Take(mainpageCount).OrderBy(p => p.ProductName).ToList();
			}

			else if (mainPageName == "Featured")
			{

				products = context.Products.Where(p => p.StatusID == 4 && p.Active == true).Take(mainpageCount).OrderBy(p => p.ProductName).ToList();
			}

			else if (mainPageName == "Notable")
			{

				products = context.Products.Where(p => p.StatusID == 5 && p.Active == true).Take(mainpageCount).OrderByDescending(p => p.UnitPrice).ToList();
				//öNCE FİYATA GÖRE TERSTEN SIRALA SONRA AYNI FİYATTA OLANLARI KENDİ İÇİNDE ADINA GÖRE SIRALA
			}

			else
			{
				products = context.Products.ToList();
			}
			
			return products;

        }

        public static bool ProductInsert(Product product)
		{
			try
			{
				//metod static oldugu icin , context burada tanımlamak zorundayım
				using (iakademi38Context context = new iakademi38Context())
				{
					product.AddDate = DateTime.Now;
					context.Add(product);
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}


		

		public static bool ProductUpdate(Product product)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Update(product);
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}


		public async Task<Product> ProductDetails(int? id)
		{
			Product? product = await context.Products.FindAsync(id);
			return product;
		}

		public  Product ProductDetails(string mainPageName)
		{
			Product? product =  context.Products.FirstOrDefault(p=>p.StatusID==6);
			return product;
		}

		public static bool ProductDelete(int id)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    Product product = context.Products.FirstOrDefault(c => c.ProductID == id);
                    product.Active = false;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }


		public static void Highlighted_Increase(int id)
		{
			using(iakademi38Context context=new iakademi38Context())
			{
				Product? product=context.Products.FirstOrDefault(p=>p.ProductID==id);//database eski kaydı getirir
				product.HighLighted += 1;//eski kaydın sadece Highlighted kolonunu 1 arttırır
				context.Update(product);//Highlighted kolonu 1 artmış olarak tekrar gunceller
				context.SaveChanges();
			}
		}


		public List<Product> ProductSelectWithCategoryID(int id)
		{
			List<Product> products = context.Products.Where(p => p.CategoryID ==id ).OrderBy(p=>p.ProductName).ToList();
			return products;
		}

		public List<Product> ProductSelectWithSupplierID(int id)
		{
			List<Product> products = context.Products.Where(p => p.SupplierID == id).OrderBy(p => p.ProductName).ToList();
			return products;
		}

		public  List<Cls_Product> SelectProductsByDetails(string query)
		{
			List<Cls_Product> products= new List<Cls_Product>();
			SqlConnection sqlConnection = Connection.ServerConnect;
			SqlCommand sqlCommand= new SqlCommand(query,sqlConnection);
			sqlConnection.Open();
			SqlDataReader sqlDataReader=sqlCommand.ExecuteReader();
			while (sqlDataReader.Read())
			{
				Cls_Product product= new Cls_Product();
				product.ProductID = Convert.ToInt32(sqlDataReader["ProductID"]);
				product.ProductName = sqlDataReader["ProductName"].ToString();
				product.UnitPrice = Convert.ToDecimal(sqlDataReader["UnitPrice"]);
				product.PhotoPath = sqlDataReader["PhotoPath"].ToString();
				products.Add(product);
			}
			return products;
		}

		public static List<Sp_arama> gettingSearchProducts(string id)
		{
			using (iakademi38Context context = new iakademi38Context())
			{
				var products=context.sp_Aramas.FromSql($"sp_arama {id}").ToList();
				return products;
			}
		}

    }
}
