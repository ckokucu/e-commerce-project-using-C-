using iakademi38_proje.Models;
using Microsoft.AspNetCore.Mvc;
using XAct;

namespace iakademi38_proje.ViewComponents
{
    public class Telephone:ViewComponent
    {
        iakademi38Context context=new iakademi38Context();

        public string Invoke()
        {
            string? telephone = context.Settings.FirstOrDefault(s => s.SettingID == 1).Telephone;
            return $"{telephone}";
        }

    }
}
