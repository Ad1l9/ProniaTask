using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaTask.Enumerations;
using ProniaTask.Models;
using ProniaTask.ViewModel;

namespace ProniaTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signIn;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> manager, SignInManager<AppUser> signIn,RoleManager<IdentityRole> roleManager)
        {
            _manager = manager;
            _signIn = signIn;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View();
            userVM.Name = userVM.Name.Trim();
            userVM.Surname = userVM.Surname.Trim();

            string name = Char.ToUpper(userVM.Name[0]) + userVM.Name.Substring(1).ToLower();
            string surname   = Char.ToUpper(userVM.Surname[0]) + userVM.Surname.Substring(1).ToLower();

            AppUser user = new()
            {
                Name = name,
                Surname = surname,
                Email = userVM.Email,
                UserName = userVM.Username,
                Gender = userVM.Gender
            };
            var result = await _manager.CreateAsync(user, userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Login(LoginVM loginVM,string? returnUrl)
		{
            if (!ModelState.IsValid) return View();
            AppUser user = await _manager.FindByNameAsync(loginVM.UsernameOrEmail);
            if(user is null)
            {
                user = await _manager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(String.Empty,"Username, Email or Password incorrect");
                    return View();
                }
            }

            var result=await _signIn.PasswordSignInAsync(user,loginVM.Password,loginVM.IsRemembered,true);


            if (result.IsLockedOut)
			{
                TimeSpan difference = (TimeSpan)(user.LockoutEnd - DateTimeOffset.Now);
				ModelState.AddModelError(String.Empty, $"Too many attempts, please try {difference.Minutes}min {difference.Seconds}sec later");
				return View();
			}
            if (!result.Succeeded)
			{
				ModelState.AddModelError(String.Empty, "Username, Email or Password incorrect");
				return View();
			}
            await _manager.AddToRoleAsync(user,UserRole.Member.ToString());
            await _signIn.SignInAsync(user, false);
			return Redirect(returnUrl);
		}

		public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        public async Task<IActionResult> CreateRoles()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if(!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new()
                    {
                        Name = role.ToString()
                    }); ;
                }
            }
            return RedirectToAction("Index","Home");
        }
    }
}
