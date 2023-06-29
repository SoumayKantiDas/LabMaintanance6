﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LabMaintanance6.Models;
using System.Net.Mail;
using System.Web.UI;
using PagedList;
using PagedList.Mvc;

namespace LabMaintanance6.Controllers.Stuff
{
    public class StuffComplainsController : Controller
    {
        private LabMaintanance4Entities db = new LabMaintanance4Entities();

        // GET: StuffComplains
        public ActionResult Index(int? i)
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }

            var complains3 = db.Complains
                .Where(c => c.status)
                .Include(c => c.AllUser)
                .Include(c => c.Priority)
                .Include(c => c.Repaired_Staus)

                .OrderByDescending(c => c.complain_id) // Order by a specific property, such as Id
                .ToList()
                .ToPagedList(i ?? 1, 3);




            return View(complains3);
        }

        // GET: StuffComplains/Details/5
        public ActionResult Details(int? id)
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Complain complain = db.Complains.Find(id);
            if (complain == null)
            {
                return HttpNotFound();
            }
            return View(complain);
        }

        // GET: StuffComplains/Create
        public ActionResult Create()
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }

            ViewBag.user_id = userId;
            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "priority1");
            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status");
            return View();
        }

        // POST: StuffComplains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Complain complain, HttpPostedFileBase imageData)
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                if (imageData != null && imageData.ContentLength > 0)
                {
                    // Convert the uploaded image to a byte array
                    byte[] imageBytes;
                    using (var binaryReader = new BinaryReader(imageData.InputStream))
                    {
                        imageBytes = binaryReader.ReadBytes(imageData.ContentLength);
                    }

                    complain.image_data = imageBytes;
                }
                complain.user_id = userId.Value;
                complain.status = true;
                db.Complains.Add(complain);
                db.SaveChanges();

                // Get the list of users with role_id = 2
                var users = db.AllUsers.Where(u => u.role_id == 2 && u.status == true).ToList();

                // Send email to each user
                foreach (var user in users)
                {
                    SendEmail(user.email, "New Complaint Created", "A new complaint has been created. Please review it.");
                }

                return RedirectToAction("Index");
            }

            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "priority1", complain.PriorityId);
            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status", complain.Repaired_StausId);
            ViewBag.user_id = userId;
            return View(complain);
        }
        private void SendEmail(string recipient, string subject, string body)
        {
            // Configure the SMTP client
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("soumaykanti2859@gmail.com", "ssatuysevdkgrthi");

            // Create the email message
            var message = new MailMessage();
            message.From = new MailAddress("soumaykanti2859@gmail.com");
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = body;

            // Send the email
            smtpClient.Send(message);
        }


        // GET: StuffComplains/Edit/5
        public ActionResult Edit(int? id)
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Complain complain = db.Complains.Find(id);
            if (complain == null)
            {
                return HttpNotFound();
            }


            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status", complain.Repaired_StausId);
            return View(complain);
        }

        // POST: StuffComplains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Complain updatedComplain)
        {
            // Retrieve user ID from session
            int? userId = Session["UserId"] as int?;
            // Retrieve role ID from session
            int? roleId = Session["RoleId"] as int?;

            // Perform authorization logic using the session's UserId and RoleId
            if (userId == null || roleId != 2)
            {
                // Authorization failed, redirect to Home/Index
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var existingComplain = db.Complains.Find(id);
                if (existingComplain == null)
                {
                    return HttpNotFound();
                }

                existingComplain.Repaired_StausId = updatedComplain.Repaired_StausId;

                db.Entry(existingComplain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status", updatedComplain.Repaired_StausId);
            return View(updatedComplain);
        }

       

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
