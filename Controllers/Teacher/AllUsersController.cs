using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LabMaintanance6.Models;

namespace LabMaintanance6.Controllers.Teacher
{
    public class AllUsersController : Controller
    {
        private LabMaintanance4Entities db = new LabMaintanance4Entities();

        // GET: AllUsers
        public ActionResult Index()
        {
            var allUsers = db.AllUsers
                .Include(a => a.Role)
                .Where(a => a.status == true); // Filter users with status = true
            return View(allUsers.ToList());
        }

        // GET: AllUsers/Delete/5
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

        // POST: AllUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllUser allUser = db.AllUsers.Find(id);
            if (allUser == null)
            {
                return HttpNotFound();
            }
            allUser.status = false; // Set the Status property to false

            db.Entry(allUser).State = EntityState.Modified;
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
