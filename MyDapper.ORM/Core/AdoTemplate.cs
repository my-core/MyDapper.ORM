using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.ORM.Core
{
    public class AdoTemplate : IAdoTemplate
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectionString = string.Empty;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbProvider dbProvider = DbProvider.MySql;
        /// <summary>
        /// 打开数据库连接()
        /// </summary>
        /// <returns></returns>
        public IDbConnection OpenConnection()
        {

            if (dbProvider == DbProvider.SqlServer)
            {
                IDbConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            else if (dbProvider == DbProvider.MySql)
            {
                IDbConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            else
            {
                IDbConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;Pooling=False;Max Pool Size=100;", ConnectionString));
                connection.Open();
                return connection;
            }
        }

        #region 通用的增/删/改/查
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Insert<T>(T t)
        {
            string sql = "insert into {0} ({1}) values ({2});";
            string cols = "";
            string vals = "";
            try
            {
                if (t != null)
                {
                    Type type = typeof(T);
                    List<PropertyInfo> propertys = TypePropertiesCache(type);
                    DbTableAttribute attribute = CustomAttributesCache(type);
                    string tableName = attribute.TableName;
                    foreach (PropertyInfo pi in propertys)
                    {
                        string name = pi.Name;
                        if (cols != "")
                        {
                            cols += ",";
                            vals += ",";
                        }
                        cols += name;
                        vals += string.Format("@{0}", name);
                    }
                    sql = string.Format(sql, tableName, cols, vals);
                    using (IDbConnection conn = OpenConnection())
                    {
                        return conn.Execute(sql, t) > 0;
                    }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t)
        {
            Type type = typeof(T);
            List<PropertyInfo> propertys = TypePropertiesCache(type);
            DbTableAttribute attribute = CustomAttributesCache(type);
            string tableName = attribute.TableName;
            string primaryKey = attribute.PrimaryKey;
            string sql = "update {0} set {1} where {2};";
            string sqlSet = string.Empty;
            string sqlWhere = string.Empty;
            try
            {
                if (t != null)
                {
                    foreach (PropertyInfo pi in propertys)
                    {
                        string name = pi.Name;
                        object value = type.GetProperty(name).GetValue(t, null);
                        //为null、时间、创建人不更新
                        if (value == null || name.ToLower() == "createtime" || name.ToLower() == "createby")
                        {
                            continue;
                        }
                        //主键(作为更新条件)
                        if (name == primaryKey)
                        {
                            sqlWhere = string.Format("{0}=@{0}", name);
                        }
                        else
                        {
                            if (sqlSet != "")
                            {
                                sqlSet += ",";
                            }
                            sqlSet += string.Format("{0}=@{0}", name);
                        }
                    }
                    sql = string.Format(sql, tableName, sqlSet, sqlWhere);
                    using (IDbConnection conn = OpenConnection())
                    {
                        return conn.Execute(sql, t) > 0;
                    }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Delete<T>()
        {
            try
            {
                DbTableAttribute attribute = CustomAttributesCache(typeof(T));
                string sql = string.Format("delete from {0} ", attribute.TableName);
                using (IDbConnection conn = OpenConnection())
                {
                    return Convert.ToInt16(conn.ExecuteScalar(sql, null, null, null, CommandType.Text)) > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public bool Delete<T, R>(R parms)
        {
            try
            {
                Type type = typeof(T);
                List<PropertyInfo> propertys = TypePropertiesCache(type);
                DbTableAttribute attribute = CustomAttributesCache(type);
                string tableName = attribute.TableName;
                List<PropertyInfo> parmsProperty = TypePropertiesCache(parms.GetType());
                StringBuilder sbSql = new StringBuilder(string.Format("delete from {0} where 1=1 ", tableName));
                foreach (PropertyInfo pi in parmsProperty)
                {
                    sbSql.AppendFormat(" and {0}=@{0}", pi.Name);
                }
                using (IDbConnection conn = OpenConnection())
                {
                    return Convert.ToInt16(conn.ExecuteScalar(sbSql.ToString(), parms, null, null, CommandType.Text)) > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public T GetModel<T, R>(R parms)
        {
            List<T> list = GetList<T, R>(parms);
            return list.Count > 0 ? list[0] : default(T);
        }

        /// <summary>
        /// 查询(多记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public List<T> GetList<T, R>(R parms)
        {
            try
            {
                Type type = typeof(T);
                List<PropertyInfo> propertys = TypePropertiesCache(type);
                DbTableAttribute attribute = CustomAttributesCache(type);
                List<PropertyInfo> parmsProperty = TypePropertiesCache(parms.GetType());
                StringBuilder sbSql = new StringBuilder(string.Format("select * from {0} where 1=1", attribute.TableName));
                //对于有IsDelete属性的，过滤已删除的数据
                if (propertys.Exists(p => p.Name.ToLower() == "isdelete"))
                {
                    sbSql.Append(" and IsDelete=0 ");
                }
                foreach (PropertyInfo pi in parmsProperty)
                {
                    sbSql.AppendFormat(" and {0}=@{0}", pi.Name);
                }
                using (IDbConnection conn = OpenConnection())
                {
                    IDataReader reader = conn.ExecuteReader(sbSql.ToString(), parms, null, null, CommandType.Text);
                    return reader.Select<T>();
                }
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public PageList<T> GetPageList<T, R>(string sql, R parms, int pageIndex, int pageSize, string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " order by " + orderBy;
            }
            int totalItemCount = Convert.ToInt32(ExecuteScalar("select count(1) from (" + sql + ") as A", parms));
            sql += " limit " + (pageIndex - 1) * pageSize + "," + pageSize + "";
            using (IDbConnection conn = OpenConnection())
            {
                IDataReader reader = conn.ExecuteReader(sql, parms, null, null, CommandType.Text);
                List<T> list = reader.Select<T>();
                return new PageList<T>(list, pageIndex, pageSize, totalItemCount);
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public PageList<T> GetPageList<T>(int pageIndex, int pageSize, string orderBy)
        {
            Type type = typeof(T);
            List<PropertyInfo> propertys = TypePropertiesCache(type);
            DbTableAttribute attribute = CustomAttributesCache(type);
            string tableName = attribute.TableName;
            string sql = "select * from " + tableName + " where 1=1";
            //对于有IsDelete属性的，过滤已删除的数据
            if (propertys.Exists(p => p.Name.ToLower() == "isdelete"))
            {
                sql += " and IsDelete=0 ";
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " order by " + orderBy;
            }
            int totalItemCount = Convert.ToInt32(ExecuteScalar("select count(1) from (" + sql + ") as A"));
            sql += " limit " + (pageIndex - 1) * pageSize + "," + pageSize + "";
            using (IDbConnection conn = OpenConnection())
            {
                IDataReader reader = conn.ExecuteReader(sql, null, null, null, CommandType.Text);
                List<T> list = reader.Select<T>();
                return new PageList<T>(list, pageIndex, pageSize, totalItemCount);
            }
        }
        #endregion

        #region ExecuteSql
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parms)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, object parms)
        {
            using (IDbConnection conn = OpenConnection())
            {
                return conn.Execute(commandText, parms, null, null, commandType);
            }
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText, object parms)
        {
            return ExecuteScalar(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, object parms)
        {
            using (IDbConnection conn = OpenConnection())
            {
                return conn.ExecuteScalar(commandText, parms, null, null, commandType);
            }
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行</returns>
        public DataRow ExecuteDataRow(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 类型/属性(反射)
        /// <summary>
        /// Domaon对象Properties集合
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis.ToList();
            }

            var properties = type.GetProperties().ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }
        /// <summary>
        /// Domaon对象自定义Attributes集合
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, DbTableAttribute> CustomAttributes = new ConcurrentDictionary<RuntimeTypeHandle, DbTableAttribute>();
        private static DbTableAttribute CustomAttributesCache(Type type)
        {
            DbTableAttribute attr;
            if (CustomAttributes.TryGetValue(type.TypeHandle, out attr))
            {
                return attr;
            }

            attr = (DbTableAttribute)type.GetCustomAttributes(typeof(DbTableAttribute), false).FirstOrDefault();
            if (attr == null)
            {
                throw new Exception("类" + type.Name + "必须添加'_TableAttribute'属性");
            }
            CustomAttributes[type.TypeHandle] = attr;
            return attr;
        }
        #endregion
    }
}
