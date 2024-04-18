using Microsoft.AspNetCore.Mvc;
using OTP_System.Models;
using BusinessLogic;
using System.Diagnostics;

namespace OTP_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Service _service;

        public HomeController(ILogger<HomeController> logger, Service service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoggedIn()
        {
            return View("LoggedIn");
        }

        [HttpPost]
        public IActionResult GenerateTOTP(long id)
        {
            try
            {
                var totp = _service.GetOtpCode(id);

                return Json(new ResponseModel()
                {
                    Value = totp,
                    Message = "Succesfully retrieved otp code"
                });
            }
            catch(Exception e)
            {
                throw new HttpRequestException("OPT Generate failed" ,e ,System.Net.HttpStatusCode.InternalServerError);
            }

        }

        [HttpPost]
        public IActionResult ValidateTOTP(string totp, int userId)
        {
            try
            {
                var validated = false;
                if (totp != null)
                {
                    validated = _service.ValidateOTP(totp, userId);
                }
                if (validated)
                {
                    return Json(new ResponseModel()
                    {
                        Success = true,
                        Value = totp,
                        Message = "Succesfully validated OTP code"
                    });
                }
                else
                {
                    return Json(new ResponseModel()
                    {
                        Success = false,
                        //Value = totp,
                        Message = "Failed to validate OTP code"
                    });
                }
            }
            catch(Exception e)
            {
                throw new HttpRequestException("OPT Generate failed" ,e , System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}