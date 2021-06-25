﻿using ElectroShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectroShop.Controllers
{
    public class SiteController : Controller
    {
        private ElectroShopDbContext db = new ElectroShopDbContext();
        // GET: Site
        public ActionResult Index(String slug = "")
        {
            int pageNumber = 1;
            Session["keywords"] = null;
            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageNumber = int.Parse(Request.QueryString["page"].ToString());
            }
            if (slug == "")
            {
                return this.Home();
            }
            else
            {
                var link = db.Links.Where(m => m.Slug == slug);
                if (link.Count() > 0)
                {
                    var res = link.First();
                    if (res.Type == "page")
                    {
                        return this.PostPage(slug);
                    }
                    else if (res.Type == "topic")
                    {
                        return this.PostTopic(slug, pageNumber);
                    }
                    else if (res.Type == "category")
                    {
                        return this.ProductCategory(slug, pageNumber);
                    }
                }
                else
                {
                    if (db.Products.Where(m => m.Slug == slug && m.Status == 1).Count() > 0)
                    {
                        return this.ProductDetail(slug);
                    }
                    else if (db.Posts.Where(m => m.Slug == slug && m.Type == "post" && m.Status == 1).Count() > 0)
                    {
                        return this.PostDetail(slug);
                    }
                }
                return this.Error(slug);
            }
        }

        public ActionResult ProductDetail(String slug)
        {
            //??
            var model = db.Products
                .Where(m => m.Slug == slug && m.Status == 1)
                .First();
            int catid = model.CateID;

            List<int> listcatid = new List<int>();
            listcatid.Add(catid);

            var list2 = db.Categorys
                .Where(m => m.ParentId == catid)
                .Select(m => m.Id)
                .ToList();
            foreach (var id2 in list2)
            {
                listcatid.Add(id2);
                var list3 = db.Categorys
                    .Where(m => m.ParentId == id2)
                    .Select(m => m.Id)
                    .ToList();
                foreach (var id3 in list3)
                {
                    listcatid.Add(id3);
                }
            }
            // danh mục cùng sản phẩm
            ViewBag.listother = db.Products
                .Where(m => m.Status == 1 && listcatid
                .Contains(m.CateID) && m.ID != model.ID)
                .OrderByDescending(m => m.Created_at)
                .Take(12)
                .ToList();
            // sản phẩm mới nhập
            ViewBag.news = db.Products
                .Where(m => m.Status == 1 /*&& listcatid.Contains(m.CatId)*/ && m.ID != model.ID)
                .OrderByDescending(m => m.Created_at).Take(4).ToList();
            return View("ProductDetail", model);
        }
        public ActionResult PostDetail(String slug)
        {
            var model = db.Posts
                 .Where(m => m.Slug == slug && m.Status == 1)
                 .First();
            int topid = model.Topid;

            List<int> listtopid = new List<int>();
            listtopid.Add(topid);

            var list2 = db.Topics
                .Where(m => m.ParentId == topid)
                .Select(m => m.Id)
                .ToList();
            foreach (var id2 in list2)
            {
                listtopid.Add(id2);
                var list3 = db.Topics
                    .Where(m => m.ParentId == id2)
                    .Select(m => m.Id)
                    .ToList();
                foreach (var id3 in list3)
                {
                    listtopid.Add(id3);
                }
            }
            // danh mục cùng bài viết
            ViewBag.listother = db.Posts
                .Where(m => m.Status == 1 && listtopid
                .Contains(m.Topid) && m.Id != model.Id)
                .OrderByDescending(m => m.Created_At)
                .Take(12)
                .ToList();

            return View("PostDetail", model);
        }
        public ActionResult Error(String slug)
        {
            return View("Error");
        }

        public ActionResult PostPage(String slug)
        {
            var item = db.Posts
                .Where(m => m.Slug == slug && m.Type == "page")
                 .First();
            return View("PostPage", item);
        }

        public ActionResult PostTopic(String slug, int pageNumber)
        {
            int pageSize = 8;
            var row_cat = db.Topics
                .Where(m => m.Slug == slug)
                .First();
            List<int> listtopid = new List<int>();
            listtopid.Add(row_cat.Id);

            var list2 = db.Topics
                .Where(m => m.ParentId == row_cat.Id)
                .Select(m => m.Id)
                .ToList();
            foreach (var id2 in list2)
            {
                listtopid.Add(id2);
                var list3 = db.Topics
                    .Where(m => m.ParentId == id2)
                    .Select(m => m.Id)
                    .ToList();
                foreach (var id3 in list3)
                {
                    listtopid.Add(id3);
                }
            }
            var list = db.Posts
                .Where(m => m.Status == 1 && listtopid.Contains(m.Topid))
                .OrderByDescending(m => m.Created_At);
            

            ViewBag.Slug = slug;
            ViewBag.CatName = row_cat.Name;
            return View("PostTopic", list
                .ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ProductCategory(String slug, int pageNumber)
        {
            int pageSize = 8;
            var row_cat = db.Categorys
                .Where(m => m.Slug == slug)
                .First();
            List<int> listcatid = new List<int>();
            listcatid.Add(row_cat.Id);

            var list2 = db.Categorys
                .Where(m => m.ParentId == row_cat.Id)
                .Select(m => m.Id)
                .ToList();
            foreach (var id2 in list2)
            {
                listcatid.Add(id2);
                var list3 = db.Categorys
                    .Where(m => m.ParentId == id2)
                    .Select(m => m.Id)
                    .ToList();
                foreach (var id3 in list3)
                {
                    listcatid.Add(id3);
                }
            }
            var list = db.Products
                .Where(m => m.Status == 1 && listcatid.Contains(m.CateID))
                .OrderByDescending(m => m.Created_at);

            ViewBag.Slug = slug;
            ViewBag.CatName = row_cat.Name;
            ViewBag.Sl = db.Products.Where(m=>m.Status!=0 && m.CateID == row_cat.Id).Count();
            return View("ProductCategory", list
                .ToPagedList(pageNumber, pageSize));
        }
        //Trang Chủ
        public ActionResult Home()
        {
            var list = db.Categorys
               .Where(m => m.Status == 1 && m.ParentId == 0)
               .Take(8)
               .ToList();
            return View("Home", list);
        }
        public ActionResult Other()
        {
            return View("Other");
        }
        //Sản phẩm trang chủ
        public ActionResult ProductHome(int catid)
        {
            List<int> listcatid = new List<int>();
            listcatid.Add(catid);

            var list2 = db.Categorys
                .Where(m => m.ParentId == catid).Select(m => m.Id)
                .ToList();
            foreach (var id2 in list2)
            {
                listcatid.Add(id2);
                var list3 = db.Categorys
                    .Where(m => m.ParentId == id2)
                    .Select(m => m.Id).ToList();
                foreach (var id3 in list3)
                {
                    listcatid.Add(id3);
                }
            }

            var list = db.Products
                .Where(m => m.Status == 1 && listcatid
                .Contains(m.CateID))
                .Take(12)
                .OrderByDescending(m => m.Created_at);

            return View("_ProductHome", list);
        }
        //Tat ca sp
        public ActionResult Product(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var list = db.Products.Where(m => m.Status == 1)
                .OrderByDescending(m => m.Created_at)
                .ToPagedList(pageNumber, pageSize);
            return View(list);
        }
        // tìm kiếm sản phẩm
        public ActionResult Search(String key, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var list = db.Products.Where(m => m.Status == 1);
            if (String.IsNullOrEmpty(key.Trim()))
            {
                return RedirectToAction("Index", "Site");
            }
            else
            {
                list = list.Where(m => m.Name.Contains(key)).OrderByDescending(m => m.Created_at);
            }
            ViewBag.Count = list.Count();
            Session["keywords"] = key;
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitContact(MContact mContact)
        {
            mContact.FullName = Request.Form["Fullname"];
            mContact.Email = Request.Form["Email"];
            mContact.Phone = Convert.ToInt32(Request.Form["Phone"]);
            mContact.Title = Request.Form["Title"];
            mContact.Detail = Request.Form["Detail"];
            mContact.Status = 1;
            mContact.Created_at = DateTime.Now;
            mContact.Updated_at = DateTime.Now;
            mContact.Updated_by = 1;

            db.Contacts.Add(mContact);
            db.SaveChanges();
            Notification.set_flash("Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. Xin cảm ơn!", "success");
            return RedirectToAction("Contact", "Site");
        }
    }
}