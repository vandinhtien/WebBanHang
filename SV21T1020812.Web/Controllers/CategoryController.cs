﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020742.BusinessLayers;
using SV21T1020742.DomainModels;
using SV21T1020742.Web.Models;
using System.Buffers;

namespace SV21T1020742.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN}, {WebUserRoles.MANAGER}")]
	public class CategoryController : Controller
	{
        public const int PAGE_SIZE = 20;
		private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
		public IActionResult Index()
		{
			PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
			var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			CategorySearchResult model = new CategorySearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue ?? "",
				RowCount = rowCount,
				Data = data
			};

			ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
			return View(model);

		}

		public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var data = new Category()
            {
                CategoryID = 0

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";

            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Tên mô tả không được để trống");

            //Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            if (data.CategoryID == 0)
            {
                int id = CommonDataService.AddCategory(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCategory(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCategory(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
