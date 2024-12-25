using SV21T1020742.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.DataLayers
{
    public interface IUserAccountDAL
    {
        //Kiểm tra xem tên đăng nhập và mật khẩu có đúng hay không
        //Nếu đúng: trả về thông tin của user, nếu sai trả về null
        UserAccount? Authorize(string username, string password);

        //Đổi mật khẩu
        bool ChangePassword(string username, string password);
    }
}
