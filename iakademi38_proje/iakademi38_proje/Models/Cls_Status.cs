using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iakademi38_proje.Models
{
    public class Cls_Status
    {
        iakademi38Context context = new iakademi38Context();
        public async Task<List<Status>> StatusSelect()
        {
            List<Status> statuses = await context.Statuses.ToListAsync();
            return statuses;
        }

		public static bool StatusInsert(Status status)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					status.StatusName.ToUpper();
					context.Add(status);
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{

				return false;
			}
		}

		public async Task<Status> StatusDetails(int? id)
		{
			//Supplier supplier=await context.Suppliers.FindAsync(id);alttaki yerine bu da olabilirdi 
			Status? status = await context.Statuses.FirstOrDefaultAsync(s => s.StatusID == id);
			return status;
		}


		public static bool StatusUpdate(Status status)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Update(status);
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}


		public static bool StatusDelete(int id)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					Status status = context.Statuses.FirstOrDefault(c => c.StatusID == id);
					status.Active = false;

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
