
using MyDapper.ORM;
using MyDapper.ORM.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.Test.Model
{
    /// <summary>
    /// 注意，Table不能缺少
    /// </summary>
    [Table("T_User", "ID", PrimaryKeyType.Assigned)]
    public class UserModel
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string RoleID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
