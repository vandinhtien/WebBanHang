using SV21T1020742.DataLayers;
using SV21T1020742.DataLayers.SQLServer;
using SV21T1020742.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        public static List<Product> ListProducts(string searchValue = "")
        {
            List<Product> products = new List<Product>();
            return products;
        }

        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
        }

        public static Product? GetProduct(int productId)
        {
            return productDB.Get(productId);
        }

        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }

        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }

        public static bool DeleteProduct(int productId)
        {
            return productDB.Delete(productId);
        }

        public static bool InUsedProduct(int productId)
        {
            return productDB.InUsed(productId);
        }

        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }

        public static ProductPhoto GetPhoto(long photoID) 
        {
            return productDB.GetPhoto(photoID);
        }

        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }

        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }

        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }

        public static List<ProductAttribute> ListAttributes(int attributeID)
        {
            return productDB.ListAttribute(attributeID).ToList();
        }

        public static ProductAttribute GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }

        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }

        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }

        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
