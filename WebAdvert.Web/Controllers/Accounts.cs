using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Accounts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly UserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public Accounts(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.pool = pool;
        }

        // GET: /<controller>/
        [HttpGet]
        [Route("Signup")]
        public IActionResult Signup()
        {
            return View(new SignupModel());
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = pool.GetUser(model.Email);
            if (user.Status != null)
            {
                ModelState.AddModelError("UserExists", "User with this email already exists");
            }

            user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

            var newUser = await userManager.CreateAsync(user, model.Password);
            if (newUser.Succeeded)
            {
                return RedirectToAction("Confirm");
            }
            return View();
        }

        [HttpGet]
        [Route("Confirm")]
        public IActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        [Route("Confirm")]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("NotFound", "User with the given email address was not found");
                return View(model);
            }

            var result = await (userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            result.Errors
                .ToList()
                .ForEach(error => ModelState.AddModelError(error.Code, error.Description));

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("LoginError", "Email and/or password do not match");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reset(ResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("UserNotFound", "User with specified email address is not found in our system");
                return View(model);
            }

            var result = await (userManager as CognitoUserManager<CognitoUser>).SendEmailConfirmationTokenAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("PasswordReset", "Cannot send reset email for this user");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
