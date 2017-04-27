
using MyDapper.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.Test.Model
{
    /// <summary>
    /// 注意，DbTable不能缺少
    /// </summary>
    [DbTable("T_User")]
    public class UserModel
    {
        public string _ID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string NickName { get; set; }
        public string MobileNo { get; set; }
        public int Sex { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
