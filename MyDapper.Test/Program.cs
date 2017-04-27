
using MyDapper.ORM.Core;
using MyDapper.Test.Model;
using MyDapper.Test.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //此处直接实例化，实际项目建议使用注入方式
            IUserService userService = new UserService();
            //插入
            for (int i = 1; i <= 10; i++)
            {
                userService.Insert(new UserModel
                {
                    _ID = Guid.NewGuid().ToString(),
                    UserName = "zhangsan_" + i.ToString(),
                    RealName = "张三_" + i.ToString(),
                    NickName = "张三_" + i.ToString(),
                    MobileNo = "13632809657",
                    Sex = 1,
                    CreateBy = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now
                });
            }
            //获取单记录
            UserModel userModel = userService.GetModel<UserModel,object>(new { UserName = "zhangsan_1" });
            //删除指定条件记录
            userService.Delete<UserModel, object>(new { UserName = "zhangsan_1" });
            //获取多记录
            List<UserModel> listUser = userService.GetList<UserModel,object>(new { UserName = "zhangsan_2" });
            //分页
            PageList<UserModel> pageList = userService.GetPageList<UserModel>(1, 5, "CreateTime");
        }
    }
}
