using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LabMaintanance6.Models;

namespace LabMaintanance6.Controllers.Teacher
{
    public class T_profileController : Controller
    {
        private LabMaintanance4Entities db = new LabMaintanance4Entities();

        // GET: T_profile
        public ActionResult Index()
        {
            var allUsers = db.AllUsers.Include(a => a.Role);
            return View(allUsers.ToList());
        }

        // GET: T_profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllUser allUser = db.AllUsers.Find(id);
            if (allUser == null)
            {
                return HttpNotFound();
            }
            return View(allUser);
        }

        // GET: T_profile/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name");
            return View();
        }

        // POST: T_profile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,username,email,role_id,password,hashPassword,status,email_verified")] AllUser allUser)
        {
            if (ModelState.IsValid)
            {
                db.AllUsers.Add(allUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name", allUser.role_id);
            return View(allUser);
        }

        // GET: T_profile/Edit/5
    
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AllUser allUser = db.AllUsers.Find(id);
            if (allUser == null)
            {
                return HttpNotFound();
            }

            return View(allUser);
        }

        // POST: T_profile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: T_profile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id, password")] AllUser allUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve user from the database
                    AllUser user = db.AllUsers.Find(allUser.user_id);

                    // Detach the existing entity from the context
                    allUser.email = user.email;
                    allUser.role_id = user.role_id;
                    allUser.username = user.username;
                    allUser.email_verified = user.email_verified;
                    allUser.status = user.status;

                    allUser.password = allUser.password;
                    // Set the modified entity state and update its properties
                    allUser.hashPassword = ComputeHash(allUser.password);

                    db.Entry(allUser).State = EntityState.Modified;
               
                    db.SaveChanges();

                    return RedirectToAction("Index", "Teacher");
                }
                catch (DbEntityValidationException ex)
                {
                    // Handle validation errors
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    // Combine the error messages into a single string
                    var errorMessage = string.Join("; ", errorMessages);

                    // Add the error message to the ModelState
                    ModelState.AddModelError("", errorMessage);
                }
            }

            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name", allUser.role_id);
            return View(allUser);
        }



        // Helper method to compute SHA hash
        private string ComputeHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // GET: T_profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllUser allUser = db.AllUsers.Find(id);
            if (allUser == null)
            {
                return HttpNotFound();
            }
            return View(allUser);
        }

        // POST: T_profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllUser allUser = db.AllUsers.Find(id);
            db.AllUsers.Remove(allUser);
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
