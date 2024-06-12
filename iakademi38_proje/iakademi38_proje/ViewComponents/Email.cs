using iakademi38_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi38_proje.ViewComponents
{
    public class Email:ViewComponent
    {
        iakademi38Context context= new iakademi38Context();

        public string Invoke()
        {
            string? email = context.Settings.FirstOrDefault(s => s.SettingID == 1).Email;
            return $"{email}";
        }
    }
}
