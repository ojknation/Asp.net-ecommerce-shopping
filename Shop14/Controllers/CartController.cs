using Shop14.Models.Data;
using Shop14.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop14.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your Cart is Empty";
                return View();
            }
            //calculate total and save to viewbag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;
            //Return View with list
            return View(cart);
        }
        public ActionResult CartPartial()
        {
            //Initialize CartVM
            CartVM model = new CartVM();

            //init Quantity
            int qty = 0;

            //init price
            decimal price = 0m;

            //Check for cart session
            if (Session["cart"] != null)
            {
                //Get total quantity and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //Set quantity and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }


            //return partial view with model
            return PartialView(model);
        }
        public ActionResult AddToCartPartial(int id)
        {
            //init cartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //init CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                //Get The product
                ProductDTO product = db.Products.Find(id);

                //Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //if not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    //if it is increment
                    productInCart.Quantity++;
                }
            }

            //Get total quantity and price and add to model
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Save cart back to session
            Session["cart"] = cart;
            //Return partial view with model
            return PartialView(model);
        }
        public JsonResult IncrementProduct(int productId)
        {
            //init cart List
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get CartVM from List
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //increment qty
                model.Quantity++;

                //Store needed data
                var result = new { qty = model.Quantity, price = model.Price };
                //Return Json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult DecrementProduct(int productId)
        {
            //init cart List
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get CartVM from List
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //Store needed data
                var result = new { qty = model.Quantity, price = model.Price };
                //Return Json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public void RemoveProduct(int productId)
        {
            //init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //remove model from list
                cart.Remove(model);
            }

        }

        public void PlaceOrder()
        {
            //Get the cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get username
            string username = User.Identity.Name;

            
            using (Db db = new Db())
            {
                //init orderDTO 
                OrderDTO orderDTO = new OrderDTO();

                int orderId = 0;

                //Get UserId
                var q = db.Users.FirstOrDefault(x => x.Username == username);
                int userId = q.Id;

                //Add to OrderDTO and Save
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO);
                db.SaveChanges();

                //Get insertedId
                 orderId = orderDTO.OrderId;

                //Init OrderDetailsDTO
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

                //Add to OrderDetails DTO
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }
            }

            //Email Admin

            //Reset Session
            Session["Cart"] = null;
        }

    }
}