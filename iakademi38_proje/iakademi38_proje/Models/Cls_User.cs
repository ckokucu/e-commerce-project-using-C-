using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text;
using XSystem.Security.Cryptography;

namespace iakademi38_proje.Models
{
    public class Cls_User
    {
        iakademi38Context context = new iakademi38Context();

        //Normal Hayatta adını RepositoryUser diye aç
        //IRepositoryUser--> interface ismi

        public async Task<User> loginControl(User user)
        {
            string md5Sifre = MD5Sifrele(user.Password);


            User? usr=await context.Users.FirstOrDefaultAsync(u=>u.Email==user.Email && u.Password==md5Sifre && u.IsAdmin==true && u.Active==true);

            return usr;
        }

        public static string MD5Sifrele(string value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] btr= Encoding.UTF8.GetBytes(value);
            btr=md5.ComputeHash(btr);

            StringBuilder sb= new StringBuilder();
            foreach (byte item in btr) 
            {
                sb.Append(item.ToString("x2").ToLower());
            }

            return sb.ToString();
        }

        public static User SelectMemberInfo(string Email)
        {
            using( iakademi38Context context=new iakademi38Context())
            {
                User user=context.Users.FirstOrDefault(u=>u.Email==Email);
                return user;
            }
            
        }

        public static string MemberControl(User user)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                string answer = "";

                try
                {
                    string md5Sifre = MD5Sifrele(user.Password);
                    User? usr = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == md5Sifre);

                    if (usr == null)
                    {
                        //kullanıcı yanlıs sifre veya emal girdi
                        answer = "error";
                    }
                    else
                    {
                        //kullanıcı veritabanında kayıtlı.
                        if (usr.IsAdmin == true)
                        {
                            //admin yetkisi olan personel giriş yapıyor
                            answer = "admin";
                        }
                        else
                        {
                            answer = usr.Email;
                        }
                    }
                }
                catch (Exception)
                {
                    return "HATA";
                }
                return answer;
            }
        }


        public static bool loginEmailControl(User user)
        {
            using(iakademi38Context context = new iakademi38Context())
            {
               User usr = context.Users.FirstOrDefault(u => u.Email == user.Email);

                if (usr == null) return false;
                 return true;
                
            }
        }

        public static bool AddUser(User user)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                try
                {
                    user.Active = true;
                    user.IsAdmin = false;
                    user.Password = MD5Sifrele(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        
        public  void Send_Sms(string OrderGroupGUID)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                string ss = "";
                ss += "<?xml version='1.0' encoding='UTF-8' >";
                ss += "<mainbody>";
                ss += "<header>";
                ss += "<company dil='TR'>iakademi(üye oldugunuzda size verilen şirket ismi)</company>";
                ss += "<usercode>0850 size verilen user kod burada yazılacak</usercode>";
                ss += "<password>NetGsm123. size verilen şifre burada yazılacak</password>";
                ss += "<startdate></startdate>";
                ss += "<stopdate></stopdate>";
                ss += "<type>n:n</type>";
                ss += "<msgheader>başlık buraya</msgeader>";
                ss += "</header>";
                ss += "<body>";

                Order order = context.Orders.FirstOrDefault(o => o.OrderGroupGUID == OrderGroupGUID);
                User user = context.Users.FirstOrDefault(u => u.UserID == order.UserID);
                //Sayın Sedat tefçi, 05 04 2023 tarihinde 5042023194420 nolu siparişiniz alınmıştır.
                string content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

                ss += "<mp><msg><![CDATA[" + content + "]]></msg><no>90" + user.Telephone + "</no></mp>";
                ss += "</body>";
                ss += "</mainbody>";

                string answer = XMLPOST("https://api.netgsm.com/tr/xmlbulkhttppost.asp", ss);
                if (answer != "-1")
                {
                    //sms gitti
                }
                else
                {
                    //sms gitmedi
                    //ilgili tabloya sms gitmedi yazdırılır(bu tabloda kayıt olmadıgı surece smsler giddiyo demektir)
                }
            }
        }

        public static string XMLPOST(string url, string xmlData)
        {
            try
            {
                WebClient wUpload = new WebClient();
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest; //Convert = CASTING
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                Byte[] bPostArray = Encoding.UTF8.GetBytes(xmlData);
                Byte[] bResonse = wUpload.UploadData(url, "POST", bPostArray);

                Char[] sReturnsChars = Encoding.UTF8.GetChars(bResonse);

                string sWebPage = new string(sReturnsChars);
                return sWebPage;
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public  void Send_Email(string OrderGroupGUID)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                Order order = context.Orders.FirstOrDefault(o => o.OrderGroupGUID == OrderGroupGUID);
                User user = context.Users.FirstOrDefault(u => u.UserID == order.UserID);

                string mail = "gonderen email buraya info@iakademi.com";
                string _mail = user.Email;//alıcı email adresi
                string subject = "";
                string content = "";

                subject = "Sayın " + user.NameSurname + " siparişiniz alınmıştır.";

                content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

               

                string host = "smtp.iakademi.com";
                int port = 587;
                string login = "mailserver a baglanılan login buraya";
                string password = "mailserver a baglanılan şifre buraya";

                MailMessage e_posta = new MailMessage();
                e_posta.From = new MailAddress(mail, "iakademi bilgi"); //gönderen
                e_posta.To.Add(_mail); //alıcı
                e_posta.Subject = subject;
                e_posta.IsBodyHtml = true;
                e_posta.Body = content;

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential(login, password);
                smtp.Port = port;
                smtp.Host = host;

                try
                {
                    smtp.Send(e_posta);
                    
                }
                catch (Exception)
                {
                    //ilgili log tablosuna email gitmedi bilgisi basarsın
                    
                }
            }
        }
    }
}
