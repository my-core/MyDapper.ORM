using MyDapper.ORM.Core;
using System;
using System.Configuration;
using System.Data;

namespace MyDapper.Test.Dao
{
    public class BaseDao : AdoTemplate
    {
        public BaseDao()
        {
            //这里配置数据库方式(这里可做成配置，根据配置加载实际项目所使用的数据库)
            //base.dbProvider = DbProvider.MySql;
            //base.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySql"];
            base.dbProvider = DbProvider.SQLite;
            base.ConnectionString = AppDomain.CurrentDomain.BaseDirectory+ System.Configuration.ConfigurationManager.AppSettings["SQLite"];
        }
    }
}
