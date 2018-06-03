using Shop14.Models.Data;
using Shop14.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop14.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Index(string page = "")
        {
            //Get/set page slut
            if (page == "")
                page = "home";
            //Declare model and DTO
            PageVM model;
            PageDTO dto;

            //Check if page exists
            using (Db db = new Db())
            {
                if (! db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            //Get Page DTO
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //Set page title 
            ViewBag.PageTitle = dto.Title;

            //Check for sidebar
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //init model
            model = new PageVM(dto);

                return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            //Declare a list of PageVM
            List<PageVM> pageVMList;

            //Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }

            //Return the partial view with list
                return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            //Declare model
            SidebarVM model;

            //Initialize model
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                model = new SidebarVM(dto);
            }
                //Return partial view with model
                return PartialView(model);
        }
    }
}