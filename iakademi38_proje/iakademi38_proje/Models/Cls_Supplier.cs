using Microsoft.EntityFrameworkCore;
using XAct;

namespace iakademi38_proje.Models
{
    public class Cls_Supplier
    {
        iakademi38Context context=new iakademi38Context();
        public async Task<List<Supplier>> SupplierSelect()
        {
            List<Supplier> suppliers = await context.Suppliers.ToListAsync();
            return suppliers;
        }


        public static bool  SupplierInsert(Supplier supplier)
        {
            try
            {
                using( iakademi38Context context = new iakademi38Context())
                {
                    supplier.BrandName.ToUpper();
                    context.Add(supplier);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public  async Task<Supplier> SupplierDetails(int? id)
        {
            //Supplier supplier=await context.Suppliers.FindAsync(id);alttaki yerine bu da olabilirdi 
            Supplier? supplier= await context.Suppliers.FirstOrDefaultAsync(s=>s.SupplierID==id);
            return supplier;
        }

        public static bool SupplierUpdate(Supplier supplier)
        {
            try
            {
                using(iakademi38Context context = new iakademi38Context())
                {
                    context.Update(supplier);
                    context.SaveChanges();
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static bool SupplierDelete(int id)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    Supplier supplier = context.Suppliers.FirstOrDefault(c => c.SupplierID == id);
                    supplier.Active = false;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}
