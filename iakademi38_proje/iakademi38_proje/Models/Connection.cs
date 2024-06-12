using Microsoft.Data.SqlClient;

namespace iakademi38_proje.Models
{
    public class Connection
    {
        public static SqlConnection ServerConnect
            {
            get
            {
                SqlConnection sqlConnection = new SqlConnection("Server=DESKTOP-TCALSD6;Database=iakademi38Core_proje;Trusted_Connection=True;TrustServerCertificate=True;");
                return sqlConnection;
            }
            }
    }
}
