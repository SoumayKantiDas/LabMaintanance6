using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LabMaintanance6.Models;

namespace LabMaintanance6.Controllers.New
{
    public class DAllUsersController : Controller
    {
        private LabMaintanance4Entities db = new LabMaintanance4Entities();

        // GET: DAllUsers
        public ActionResult Index()
        {
            var allUsers = db.AllUsers.Include(a => a.Role);
            return View(allUsers.ToList());
        }

        public ActionResult Getsession()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StoreEmailInSession(string email)
        {
            // Check if the email is not empty (you can add further validation if needed)
            if (!string.IsNullOrEmpty(email))
            {
                // Store the email in the session
                Session["UserEmail"] = email;
            }

            // Redirect back to the Getsession action
            return RedirectToAction("Getsession");
        }

        // GET: DAllUsers/Details/5
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

        // GET: DAllUsers/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name");
            return View();
        }

        // POST: DAllUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,username,email,role_id,password,hashPassword,status,email_verified,ResetToken,ResetTokenExpiry,passwordConfirmation")] AllUser allUser)
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

        // GET: DAllUsers/Edit/5
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
            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name", allUser.role_id);
            return View(allUser);
        }

        // POST: DAllUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,username,email,role_id,password,hashPassword,status,email_verified,ResetToken,ResetTokenExpiry,passwordConfirmation")] AllUser allUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.Roles, "role_id", "role_name", allUser.role_id);
            return View(allUser);
        }

        // GET: DAllUsers/Delete/5
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

        // POST: DAllUsers/Delete/5
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
