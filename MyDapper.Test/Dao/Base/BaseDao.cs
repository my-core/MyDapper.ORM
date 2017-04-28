using MyDapper.ORM;
using MyDapper.ORM.Generator;
using MyDapper.Test.Common;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace MyDapper.Test.Dao
{
    public class BaseDao : AdoTemplate, IAdoTemplate
    {
        public BaseDao()
        {
            DbConnection = new MySqlConnection(Utils.GetConfig("MySql",""));
            SqlGenerator = new MySqlGenerator();
        }
    }
}
