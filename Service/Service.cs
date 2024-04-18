using EntityFramework;
using EntityFramework.Models;
using Microsoft.IdentityModel.Protocols;
using OtpNet;
using System.Configuration;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using TOTP;

namespace BusinessLogic
{
    public class Service
    {
        private Totp _otp;
        private string _otpCode;
        private readonly OtpsystemContext _context;
        private readonly TOTP.TOTP _totp;
        public Service(OtpsystemContext otpsystemContext, TOTP.TOTP totp)
        {
            _context = otpsystemContext;
            _totp = totp;

        }

        public string GetOtpCode(long id)
        {
            if(_otpCode == null)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserId == id);
                _otp = _totp.GenerateTOTP(user.PasswordHash);
                _otpCode = _totp.GenerateOTPCode(_otp);

                string hash = QuickHash(_otpCode);

                if (user != null)
                {
                    user.PasswordHash = hash;
                    _context.SaveChanges();
                }

                return _otpCode;
            }
            return _otpCode;
        }
        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.UserId == id);
        }

        public bool ValidateOTP(string otpCode, int userId)
        {
            var user = GetUserById(userId);

            string hash = QuickHash(otpCode);
            if (user != null)
            {
                if(user.PasswordHash != hash) 
                {
                    return false;
                }
            }

            return true;
        }
        private static string QuickHash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }

    }
}