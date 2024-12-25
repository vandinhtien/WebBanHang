using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020742.BusinessLayers;
using SV21T1020742.DomainModels;
using SV21T1020742.Web.Models;
using System.Buffers;
using System.Globalization;

namespace SV21T1020742.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN}")]

    public class EmployeeController : Controller
	{
        public const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);

        }

        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);

        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Employee data, string _birtDate, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            if(string.IsNullOrWhiteSpace(data.FullName))
                 ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "  Vui lòng nhập Email của nhân viên");

            if (string.IsNullOrWhiteSpace(_birtDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh của nhân viên");


            DateTime? d = _birtDate.ToDateTime();
            if (d != null)
            {
                // Kiểm tra xem ngày sinh có nằm trong khoảng ngày 1/1/1753 đến ngày hiện tại hay không
                if (d.Value < new DateTime(1753, 1, 1))
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh hợp lệ (từ ngày 1 tháng 1 năm 1753 trở về sau).");
                }
                else if (d.Value > DateTime.Now)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng chọn năm sinh không vượt quá ngày hiện tại.");
                }
                else
                {
                    data.BirthDate = d.Value;
                }
            }    
            else
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh nhập không hợp lệ");

            if (string.IsNullOrWhiteSpace(data.Phone))
                data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Address))
                data.Address = "";

            if(!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            // xử lý ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;

            }

            if (data.EmployeeID == 0)
            {
                int id  = CommonDataService.AddEmployee(data);
                if(id  <= 0 )
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetEmployee(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
