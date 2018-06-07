using Shop14.Models.Data;
using Shop14.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop14.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare List of CategoryVM
            List<CategoryVM> categoryVMList;

            //init the list
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
                //Return Partial with List
                return PartialView(categoryVMList);
        }

        public ActionResult Category(string name)
        {
            //Declare a list of productVM
            List<ProductVM> productVMLIst;

            using (Db db = new Db())
            {
                //Get CategoryId
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                //Initialize the list
                productVMLIst = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                //Get Category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;

                if(productCat == null)
                {
                      return View(productVMLIst);

                }
            }

            //Return view with list
            return View(productVMLIst);
        }

        //GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare the VM and DTO
            ProductVM model;
            ProductDTO dto;
            //init product Id
            int id = 0;

            using (Db db = new Db())
            {
                //Check if products exists
                if (! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }
                //init product DTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get inserted Id
                id = dto.Id;

                //Init model
                model = new ProductVM(dto);
            }

            //Get Gallery Images 
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fileName => Path.GetFileName(fileName));
            //Return the view with model
            return View("ProductDetails", model);
        }
    }
}