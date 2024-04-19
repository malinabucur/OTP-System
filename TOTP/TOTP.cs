using OtpNet;
using System.Security.Cryptography;
using System.Text;

namespace TOTP
{
    public class TOTP 
    {
        public Totp GenerateTOTP(string secretKey)
        {
            var correction = new TimeCorrection(DateTime.UtcNow);
            byte[] bytes = Encoding.ASCII.GetBytes(secretKey);
            var totp = new Totp(bytes, 30, timeCorrection: correction);

            return totp;
        }
        public string GenerateOTPCode(Totp otp)
        {
            var totpCode = otp.ComputeTotp(DateTime.UtcNow);

            return totpCode;
        }
    } 
}