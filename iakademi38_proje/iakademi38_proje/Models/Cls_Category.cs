using Microsoft.EntityFrameworkCore; //namespace

namespace iakademi38_proje.Models
{
    public class Cls_Category
    {
        //RepositoryCategory olcak normalde bu 
        //RepositoryCategory:IRepositoryCategory

        iakademi38Context context = new iakademi38Context();

        public async Task<List<Category>>CategorySelect()
        {
            List<Category> categories = await context.Categories.ToListAsync();
            return categories;
        }


        public List<Category> CategorySelectMain()
        {
            List<Category> categories=context.Categories.Where(c=>c.ParentID==0).ToList();
            return categories;
        }

        public static bool  CategoryInsert(Category category)
        {
            try
            {
                //metod statik olduğundan contexti burada tanımlamak zorundayız
                using (iakademi38Context context=new iakademi38Context())
                {
                    context.Add(category);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Category> CategoryDetails(int? id)
        {
            Category? categories = await context.Categories.FindAsync(id); //mümkün mertebe FindFirstOrDefaultAsync kullan
            return categories;
        }



        public static bool CategoryUpdate(Category category)
        {
            try
            {
				//metod statik olduğubdan context almak zorundayım
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Update(category);
					context.SaveChanges();
					return true;
				}
			}
            catch (Exception)
            {
                return false;
            }
            

        }


        public static bool CategoryDelete(int id)
        {

            try
            {
                using (iakademi38Context context = new iakademi38Context())
                { 
                        Category category = context.Categories.FirstOrDefault(c => c.CategoryID == id);
                    category.Active = false;

                    List<Category> categoryList = context.Categories.Where(c => c.ParentID == id).ToList();
                    foreach (var item in categoryList)
                    {
                        item.Active = false;
                    }

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
