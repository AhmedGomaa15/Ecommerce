using System;
using System.Configuration;
using System.IO;

namespace Ecommerce
{
    public class Utils
    {
        public static string getConnection()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }

        public static bool isValidExtension(string fileName)
        {
            string[] validExtensions = { ".jpg", ".jpeg", ".png" };
            string extension = Path.GetExtension(fileName).ToLower();
            return Array.Exists(validExtensions, ext => ext == extension);
        }


        public static string getUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

public static string getImageUrl(object url)
{
    if (url == null || url == DBNull.Value || string.IsNullOrEmpty(url.ToString()))
    {
        return "~/Images/No_image.png";  // Fallback image when no image URL is provided
    }

    // Ensure we return the correct relative path
    return "~/Images/Category/" + Path.GetFileName(url.ToString());
}






    }
}

