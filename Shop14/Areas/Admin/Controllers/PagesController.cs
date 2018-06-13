using Shop14.Models.Data;
using Shop14.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop14.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> PagesList;

            using (Db db = new Db())
            {
                PagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            return View(PagesList);
        }
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //declare slug
                string slug;

                //init pageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                dto.Title = model.Title;

                //check slugs
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or slug already exists.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //save Dto
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //set TempData Message

            TempData["SM"] = "Page Added Successfully!";
            return RedirectToAction("AddPage");
        }

        //GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare PageVM
            PageVM model;

            using(Db db = new Db())
            {
                //GEt the page
                PageDTO dto = db.Pages.Find(id);

                //Confirm if page exists
                if(dto == null)
                {
                    return Content("This page does not exist");
                }
                //initialize pageVM
                model = new PageVM(dto);
            }
            return View(model);
        }

        //POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)

        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db = new Db())
            {

                //set page id
                int id = model.Id;
                //Declare slug
                string slug = "home";
                //Get the page 
                PageDTO dto = db.Pages.Find(id);
                //DTO the title
                dto.Title = model.Title;
                //check for slug and set it if need be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();

                    }
                }
                //make sure title and slug are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Slug or title already exists.");
                    return View(model);
                }
                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save DTO changes
                db.SaveChanges();

            }

            //set Tempdata message
            TempData["SM"] = "Page edited successfully";

            //redirect
            return RedirectToAction("EditPage");
        }
        //GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM model;
            
            using (Db db = new Db())
            {
                //GET the page
                PageDTO dto = db.Pages.Find(id);
                //confirm page exists
                if (dto == null)
                {
                    return Content("This page does not exist");
                }
                //init pageVM
                model = new PageVM(dto);
            }
            //return page with view
            return View(model);
        }
        //GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //Remove page
                db.Pages.Remove(dto);
                //save
                db.SaveChanges();
            }
            //Redirect
            return RedirectToAction("Index");
        }
        //POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using(Db db = new Db())
            {
                //set initial count
                int count = 1;

                //declare page DTO
                PageDTO dto;

                //set sorting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        } 
        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare the model
            SidebarVM model;

            using (Db db = new Db())
            {
                //get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //init model
                model = new SidebarVM(dto);
            }
            //return the model with view
            return View(model);
        }
        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using( Db db = new Db())
            {

                //get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //DTO the Body
                dto.Body = model.Body;

                //save
                db.SaveChanges();
            }

            //set TempData message
            TempData["SM"] = "Sidebar edited successfully";

            //Redirect
            return RedirectToAction("EditSidebar");
        }
    }
}