using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020742.BusinessLayers;
using SV21T1020742.DomainModels;
using SV21T1020742.Web.Models;
using System.Globalization;

namespace SV21T1020742.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.SALE}")]

    public class OrderController : Controller
	{
		public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
		public const int PAGE_SIZE = 20;

		private const int PRODUCT_PAGE_SIZE = 5;
		private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
		private const string SHOPPING_CART = "ShoppingCart";

		public IActionResult Index()
		{
			var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
			if(condition == null)
			{
				var cultureInfo = new CultureInfo("en-GB");
				condition = new OrderSearchInput()
				{
					Page = 1,
					PageSize = PAGE_SIZE,
					SearchValue = "",
					Status = 0,
					TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
				};
			}
			return View(condition);
		}

		public IActionResult Search(OrderSearchInput condition)
		{
			int rowCount;
			var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
			var model = new OrderSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue ?? "",
				Status = condition.Status,
				TimeRange = condition.TimeRange,
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
			return View(model);
		}

		public IActionResult Create()
		{
			var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
			if(condition == null)
			{
				condition = new ProductSearchInput()
				{
					Page = 1,
					PageSize = PRODUCT_PAGE_SIZE,
					SearchValue = ""
				};
			}
			return View(condition);
		}

		public IActionResult SearchProduct(ProductSearchInput condition)
		{
			int rowCount = 0;
			var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			var model = new ProductSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue ?? "",
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
			return View(model);
		}

		public IActionResult Details(int id =0 )
		{
			var order = OrderDataService.GetOrder(id);
			if(order == null)
				return RedirectToAction("Index");

			var details = OrderDataService.ListOrderDetails(id);
			var model = new OrderDetailModel()
			{
				Order = order,
				Details = details
			};
			return View(model);
		}

		public IActionResult EditDetail(int id=0, int productId = 0)
		{
            var data = OrderDataService.GetOrderDetail(id, productId);
            if (data == null)
            {
                return RedirectToAction("Details", new { id = id });
            }
            return View(data);
        }

        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            OrderDataService.DeleteOrderDetail(id, productID);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public IActionResult UpdateDetail(OrderDetail orderDetail)
        {
            OrderDataService.SaveOrderDetail(orderDetail.OrderID, orderDetail.ProductID, orderDetail.Quantity, orderDetail.SalePrice);
            return RedirectToAction("Details", new { id = orderDetail.OrderID });
        }

        private List<CartItem> GetShoppingCart()
		{
			var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
			if(shoppingCart == null)
			{
				shoppingCart = new List<CartItem>();
				ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			}
			return shoppingCart;
		}

        [HttpPost]
        public IActionResult Shipping(Order order)
        {
            if (order.ShipperID == 0)
            {
                ModelState.AddModelError(nameof(order.ShipperID), "Vui lòng chọn nhà giao hàng");
                //return View("Shipping", order);
            }
            else
                OrderDataService.ShipOrder(order.OrderID, order.ShipperID.Value);
            return RedirectToAction("Details", new { id = order.OrderID });

        }
        public IActionResult Shipping(int id = 0)
        {
            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Details", new { id = id });
            }
            return View(data);
        }
        public IActionResult Accept(int id = 0)
        {
            OrderDataService.AcceptOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Finish(int id = 0)
        {
            OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Cancel(int id = 0)
        {
            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Reject(int id = 0)
        {
            OrderDataService.RejectOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Delete(int id = 0)
        {
            OrderDataService.DeleteOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult AddToCart(CartItem item)
		{
			if (item.SalePrice < 0 || item.Quantity <= 0)
				return Json("Giá bán và số lượng không hợp lệ");

			var shoppingCart = GetShoppingCart();
			var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
			if (existsProduct == null)
			{
				shoppingCart.Add(item);
			}
			else
			{
				existsProduct.Quantity += item.Quantity;
				existsProduct.SalePrice = item.SalePrice;
			}
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			return Json("");
		}

		public IActionResult RemoveFromCart(int id = 0)
		{
			var shoppingCart = GetShoppingCart();
			int index = shoppingCart.FindIndex(m => m.ProductID == id);
			if (index >= 0)
				shoppingCart.RemoveAt(index);
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			return Json("");
		}

		public IActionResult ClearCart()
		{
			var shoppingCart = GetShoppingCart();
			shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

		public IActionResult ShoppingCart()
		{
			return View(GetShoppingCart());
		}

		public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            var userData = User.GetUserData();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống, vui lòng chọn mặt hàng cần bán");
            }
            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            }

			int employeeID = int.Parse(userData.UserId);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
			
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
    }
}
