using Azure;
using Dapper;
using SV21T1020742.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SV21T1020742.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                                FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                                ) AS t
                            WHERE (@PageSize = 0)
                                OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                    categoryID,
                    supplierID,
                    minPrice,
                    maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue.Trim()}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)";
                var parameters = new
                {
                    searchValue,
                    categoryID,
                    supplierID,
                    minPrice,
                    maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return count;
        }

        public int Add(Product data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                            values (@ProductName, @ProductDescription,@SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                            select SCOPE_IDENTITY();";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling

                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }


            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributes where DisplayOrder = @DisplayOrder)
                                select -1;
                            else
                                begin
                                    insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                    values (@ProductID, @AttributeName,@AttributeValue, @DisplayOrder);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductPhotos where DisplayOrder = @DisplayOrder)
                                select -1;
                            else
                                begin
                                    insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    values (@ProductID, @Photo,@Description, @DisplayOrder, @IsHidden);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // Câu lệnh SQL xóa dữ liệu trong ProductPhoto trước
                var sqlDeletePhotos = @"DELETE FROM ProductPhotos WHERE ProductID = @ProductID";
                var sqlDeleteAttributes = @"DELETE FROM ProductAttributes WHERE ProductID = @ProductID";
                var sqlDeleteProduct = @"DELETE FROM Products WHERE ProductID = @ProductID";

                var parameters = new { ProductID = productID };

                // Thực thi lệnh xóa ở bảng ProductPhotos
                connection.Execute(sql: sqlDeletePhotos, param: parameters, commandType: CommandType.Text);

                // Thực thi lệnh xóa ở bảng ProductAttributes
                connection.Execute(sql: sqlDeleteAttributes, param: parameters, commandType: CommandType.Text);

                // Thực thi lệnh xóa ở bảng Products
                result = connection.Execute(sql: sqlDeleteProduct, param: parameters, commandType: CommandType.Text) > 0;

            }

            return result;
        }


        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM ProductAttributes
                            WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM ProductPhotos
                            WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from OrderDetails where ProductID = @ProductID) select 1
                            else select 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return result;
        }

        

        public IList<ProductAttribute> ListAttribute(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM ProductAttributes
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM ProductPhotos
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Products
                                set ProductName = @ProductName,
                                    ProductDescription = @ProductDescription,
                                    SupplierID = @SupplierID,
                                    CategoryID = @CategoryID,
                                    Unit = @Unit,
                                    Price = @Price,
                                    Photo = @Photo,
                                    IsSelling = @IsSelling
                                where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributeS where AttributeID <> @AttributeID and ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1;
                            else
                                begin
                                    update ProductAttributeS
                                    set ProductID = @ProductID,
                                        AttributeName = @AttributeName,
                                        AttributeValue = @AttributeValue,
                                        DisplayOrder = @DisplayOrder
                                    where AttributeID = @AttributeID
                                end";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductPhotos where PhotoID <> @PhotoID and ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1;
                            else
                                begin
                                    update ProductPhotos
                                    set ProductID = @ProductID,
                                        Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                    where PhotoID = @PhotoID
                                end";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden 

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
