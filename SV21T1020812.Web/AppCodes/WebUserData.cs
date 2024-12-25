using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020742.Web.AppCodes
{
    //Những thông tin dùng để bieru diễn/ mô tả "danh tính" của người dùng
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }

        //Tạo ra chứng nhận để ghi nhận danh tính của người dùng
        public ClaimsPrincipal CreatePrincipal()
        {
            // Tạo danh sasfch các claim, mỗi claim lưu giữ 1 thông tin trong danh tính người dùng
            List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Photo), Photo),
            };
            if(Roles != null)
            {
                foreach(var role in Roles) { 
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            //Tạo Identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Tạo pricipal(giáy chứng nhận)
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
