using MyDapper.ORM.Core;

using System.Configuration;
using System.Data;

namespace MyDapper.Test.Dao
{
    public class BaseDao : AdoTemplate
    {
        public BaseDao()
        {
            //这里配置数据库方式
            base.dbProvider = DbProvider.MySql;
            base.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySql"];
        }
    }
}
