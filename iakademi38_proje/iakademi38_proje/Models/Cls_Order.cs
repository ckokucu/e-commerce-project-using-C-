using XAct;

namespace iakademi38_proje.Models
{
    public class Cls_Order
    {

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public string? MyCart { get; set; } //10=1&20=1&30=1

        public decimal UnitPrice { get; set; }

        public string? ProductName { get; set; }

        public int Kdv { get; set; }

        public string? PhotoPath { get; set; }

        public string? tckimlik_vergi_no { get; set; }


        iakademi38Context context=new iakademi38Context();

		#region  Sepete Ekle
		//sepete ekle
		//projede herhangi bir sayfada sepete ekle kullanınca buraya gelcez
		#endregion

		public bool AddToMyCart(string id)
		{
			bool exists = false;
			if (MyCart == "")
			{
				//sepete ilk defa ürün eklicek
				MyCart = id + "=1";
			}

			else
			{
				string[] MyCartArray = MyCart.Split("&");//ürünleri birbirinden ayırdım 
				for (int i = 0; i < MyCartArray.Length; i++)
				{
					string[] MyCartArrayLoop = MyCartArray[i].Split('=');
					if (MyCartArrayLoop[0]==id)
					{
						exists = true;//ürün daha önce sepete eklenmiş
					}
				}

				if (exists==false)
				{
					MyCart=MyCart+"&"+id.ToString()+"=1";
				}
			}

			return exists;
		}


		public void DeleteFromMyCart(string id)
		{
			string[] MyCartArray = MyCart.Split("&");
			string NewMyCart = "";
			int count = 1;//birden fazla ürün için & işareti kullanacgım

			for (int i = 0; i < MyCartArray.Length; i++)
			{
				string[] MyCartArrayLoop = MyCartArray[i].Split('=');
				//for her dondugunde dızının sıfırıncı alanındaki degeri (10,20,30----ProductID) MyCartID ye atadım
				string MyCartID = MyCartArrayLoop[0];

				if (MyCartID != id)
				{
					//silinmeyecek olan ürünleri yeni sepete at
					if (count==1)
					{
						//sepette ilk  ürün eklenirken & işareti olmaz
						NewMyCart = MyCartArrayLoop[0] + "=" + MyCartArrayLoop[1];
						count++;
					}
					else 
					{
						//sepete ilk üründen sonraki ürünleri eklerken basında & işareti olur
						NewMyCart+="&"+ MyCartArrayLoop[0] + "=" + MyCartArrayLoop[1];
					}
				}
				

			}

			MyCart = NewMyCart;


		}


		public List<Cls_Order> SelectMyCart() //List<Cls_Order> dediğimiz için propertyler donus yapacak
		{
			List<Cls_Order> list=new List<Cls_Order>();
			string[] MyCartArray = MyCart.Split('&');

			if (MyCartArray[0]!="")
			{
				for (int i = 0; i < MyCartArray.Length; i++)
				{
					string[] MyCartArrayLoop = MyCartArray[i].Split('=');
					int MyCartID = Convert.ToInt32(MyCartArrayLoop[0]);

					Product? prd = context.Products.FirstOrDefault(p => p.ProductID == MyCartID);

					//veri tabanındaki verileri propertylere yazdırıyorum
					Cls_Order ord = new Cls_Order();
					ord.ProductID=prd.ProductID;
                    ord.Quantity = Convert.ToInt32(MyCartArrayLoop[1]);
                    ord.UnitPrice = prd.UnitPrice;
                    ord.ProductName = prd.ProductName;
					ord.PhotoPath = prd.PhotoPath;
					ord.Kdv = prd.Kdv;
					list.Add(ord);
				}
			}

			return list;
		}




        public void EfaturaCreate()
        {
            //digital planet xml dosyası
        }

        public string OrderCreate(string Email)
        {

            List<Cls_Order> sipList = SelectMyCart();
			//14:01:2024 12.46.35.123
            string OrderGroupGUID = DateTime.Now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "");
            DateTime OrderDate = DateTime.Now; ;

            foreach (var item in sipList)
            {
                Order order = new Order();
                order.OrderDate = OrderDate;
                order.OrderGroupGUID = OrderGroupGUID;
                order.UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;
                order.ProductID = item.ProductID;
                order.Quantity = item.Quantity;
                context.Orders.Add(order);
                context.SaveChanges();
            }
            return OrderGroupGUID;
        }



        public List<vw_MyOrders> SelectMyOrders(string Email)
        {
            int UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;

            List<vw_MyOrders> myOrders = context.vw_MyOrders.Where(o => o.UserID == UserID).ToList();

            return myOrders;
        }


    }



}
