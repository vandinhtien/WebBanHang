using SV21T1020742.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.DataLayers
{
    public interface IOrderDAL
    {
        IList<Order> List(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");

        int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");

        Order? Get(int orderID);

        int Add(Order data);

        bool Update(Order data);

        bool Delete(int orderID);

        IList<OrderDetail> ListDetails(int orderID);

        OrderDetail? GetDetail(int orderID, int productID);

        bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice);

        bool DeleteDetail(int orderID, int productID);

    }
}
