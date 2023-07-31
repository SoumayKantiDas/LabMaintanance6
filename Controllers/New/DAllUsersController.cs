using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            string userEmail = Session["UserEmail"] as string;
            var allUsers = db.AllUsers.Where(u => u.email == userEmail);
            return View(allUsers.ToList());
        }
        //public ActionResult Index()
        //{
        //    // Get the email from the session
        //    string userEmail = Session["UserEmail"] as string;

        //    // Query the database to get the users based on the email filter
        //    var usersQuery = db.AllUsers.Include(a => a.Role);

        //    // Check if the email is not empty
        //    if (!string.IsNullOrEmpty(userEmail))
        //    {
        //        // Filter users with the matching email
        //        usersQuery = usersQuery.Where(u => u.email == userEmail);
        //    }

        //    // Retrieve the users from the database
        //    var users = usersQuery.ToList();

        //    return View(users);
        //}
        public async Task<ActionResult> SendEmail(string email, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("soumaykanti2859@gmail.com", "ssatuysevdkgrthi"),
                    EnableSsl = true
                };

                using (var message = new MailMessage("soumaykanti2859@gmail.com", email)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtpClient.SendMailAsync(message);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exception and display appropriate error message
                ModelState.AddModelError($"{ex}", "Failed to send email. Please try again later.");
                return RedirectToAction("Index");
            }
        }



        public async Task<ActionResult> VerifyEmail(int? id)
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

            // Generate and send a new verification OTP to the user's email
            int verificationOTP = GenerateOTP(); // Replace this with your logic to generate the OTP
            string emailBody = $"Your verification OTP is: {verificationOTP}";
            await SendEmail(allUser.email, "Email Verification OTP", emailBody);

            // Save the OTP and the registered email in the session for verification
            Session["EmailVerificationOTP"] = verificationOTP;
            Session["RegisteredEmail"] = allUser.email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyEmail(string otp)
        {
            var enteredOTP = int.Parse(otp);
            var savedOTP = (int)Session["EmailVerificationOTP"];
            if (enteredOTP == savedOTP)
            {
                var email = Session["RegisteredEmail"].ToString();

                var user = db.AllUsers.SingleOrDefault(u => u.email == email);
                if (user != null)
                {
                    user.status = true;
                    user.email_verified = true;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Email verified successfully!";
                return RedirectToAction("Edit", new { id = user.user_id });
            }

            ModelState.AddModelError("", "Invalid OTP. Please try again.");
            return View();
        }

        // Helper method to generate a random 6-digit OTP (you can implement your own logic here)
        private int GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }



        public ActionResult StoreEmailInSession()
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
                return RedirectToAction("Index","DAllUsers");
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
            //// Check if the user is authenticated and the session contains the email
            //if (!User.Identity.IsAuthenticated || Session["UserEmail"] == null)
            //{
            //    // Redirect to the Index page in the Home controller if the user is not authenticated or session is empty
            //    return RedirectToAction("Index", "Home");
            //}

            // Get the email from the session
            string userEmail = Session["UserEmail"] as string;

            // Retrieve the user with the matching email from the database
            AllUser user = db.AllUsers.SingleOrDefault(u => u.email == userEmail);

            // Check if the user with the specified ID exists
            if (user == null || id == null)
            {
                return HttpNotFound();
            }

            // Check if the db session password matches the existing session password
            // Replace 'sessionPassword' with the actual password property from the 'AllUser' model
            if (user.email != Session["UserEmail"] as string)
            {
                // Redirect to the Index page in the Home controller if passwords do not match
                return RedirectToAction("Index", "Home");
            }

            
            return View();
        }

        // POST: DAllUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, AllUser updatedPassword)
        {
           

            if (ModelState.IsValid)
            {

                var existingpassword = db.AllUsers.Find(id);
                
                if (existingpassword == null)
                {
                    return HttpNotFound();
                }
                existingpassword.password = updatedPassword.password;
                existingpassword.hashPassword = ComputeHash(updatedPassword.password);
                db.Entry(existingpassword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(updatedPassword);
        }
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
