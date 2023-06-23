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
namespace LabMaintanance6.Controllers.Teacher
{
    public class ComplainsController : Controller
    {
        private LabMaintanance4Entities db = new LabMaintanance4Entities();

        // GET: Complains
        public ActionResult Index()
        {
            var complains = db.Complains
                .Where(c => c.status)
                .Include(c => c.AllUser)
                .Include(c => c.Priority)
                .Include(c => c.Repaired_Staus)
                .ToList();

            return View(complains);
        }


        // GET: Complains/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: Complains/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.AllUsers, "user_id", "username");
            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "priority1");
            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status");
            return View();
        }

        // POST: Complains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Complain complain, HttpPostedFileBase imageData)
        {
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
                complain.status = true;
                db.Complains.Add(complain);
                db.SaveChanges();

                // Get the list of users with role_id = 2
                var users = db.AllUsers.Where(u => u.role_id == 2).ToList();

                // Send email to each user
                foreach (var user in users)
                {
                    SendEmail(user.email, "New Complaint Created", "A new complaint has been created. Please review it.");
                }

                return RedirectToAction("Index");
            }

            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "priority1", complain.PriorityId);
            ViewBag.Repaired_StausId = new SelectList(db.Repaired_Staus, "Repaired_StausId", "R_Status", complain.Repaired_StausId);
            ViewBag.user_id = new SelectList(db.AllUsers, "user_id", "username", complain.user_id);
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


        // GET: Complains/Edit/5
        public ActionResult Edit(int? id)
        {
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

        // POST: Complains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Complains/Edit/5

       
        public ActionResult Edit(int id, Complain updatedComplain)
        {
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




        // GET: Complains/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Complains/Delete/5

        // POST: Complains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Complain complain = db.Complains.Find(id);
            if (complain == null)
            {
                return HttpNotFound();
            }

            complain.status = false; // Set the Status property to false

            db.Entry(complain).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
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