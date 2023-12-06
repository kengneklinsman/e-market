using Emarketing.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Emarketing.ViewModels;
using PagedList;
using System;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Collections.Generic;
using System.IO;


namespace Emarketing.Controllers
{
    public class ClientController : Controller
    {
        dbemarketingEntities db = new dbemarketingEntities();
        private readonly dbemarketingEntities _dbContext;

        public ClientController()
        {
            _dbContext = new dbemarketingEntities();
        }
        public ClientController(dbemarketingEntities dbContext)
        {
            _dbContext = dbContext;
        }


        //GET: Client

        public ActionResult Index()
        {
            int pagesize = 9, pageindex = 1;
            //pageindex = page.HasValue ? Convert.ToInt32(Page) : 1;
            var list = db.tbl_category.OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            // Your logic to display client-specific information
            return View(stu);
        }

        //GET: Client/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Client/Login
        [HttpPost]
        [ActionName("Login")]
        public ActionResult login(tbl_client avm)
        {
            tbl_client ad = db.tbl_client.Where(x => x.cl_name == avm.cl_name && x.email == avm.email).SingleOrDefault();
            if (ad != null)
            {

                Session["cl_id"] = ad.cl_id.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Invalid username or Email";

            }

            return View();
        }

        // GET: Client/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        //POST: Client/SignUp
       [HttpPost]
        public ActionResult SignUp(tbl_client uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_client u = new tbl_client();
                u.cl_name = uvm.cl_name;
                u.email = uvm.email;
                u.phone = uvm.phone;
            }
            return View();
        }


        // GET: Client/Products
        [AllowAnonymous]
        public ActionResult Products()
        {
            // Retrieve a list of products from users
            var products = _dbContext.tbl_product.ToList();
            return View(products);
        }

        // GET: Client/ViewProduct/5
        public ActionResult ViewProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = _dbContext.tbl_product.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
        [HttpGet]
        public ActionResult CreateAd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");


            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.pro_image = path;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_des = pvm.pro_des;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");

            }

            return View();
        }
        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }


        public ActionResult ViewAd(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;




            return View(ad);
        }


        // POST: Client/PurchaseProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseProduct(int id)
        {
            try
            {
                var product = _dbContext.tbl_product.Find(id);

                if (product == null)
                {
                    return HttpNotFound();
                }

                // Your logic for handling the purchase, e.g., updating inventory, creating a purchase record, etc.

                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                // Log the exception to get more details
                Console.WriteLine(ex.ToString());

                // You might want to handle or display the exception differently in a real application
                return View("Error");
            }
        }
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}