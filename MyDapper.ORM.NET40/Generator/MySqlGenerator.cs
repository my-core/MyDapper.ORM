using MyDapper.ORM.Mapper;
using MyDapper.ORM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDapper.ORM.Generator
{
    public class MySqlGenerator : SqlGenerator, ISqlGenerator
    {
        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql<T>(int pageIndex, int pageSize, string orderBy)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format("SELECT * FROM {0} LIMIT {1},{2}", mapT.TableName, (pageIndex - 1) * pageSize, pageSize);
        }

        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W">查询对象</typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql<T, W>(W where, int pageIndex, int pageSize, string orderBy)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            ClassMapper mapW = GetMapper(where.GetType());
            string strWhere = mapW.Properties.Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("SELECT * FROM {0} WHERE {1} {2} ORDER BY {3} LIMIT {4},{5}", mapT.TableName, EmptyExpression, strWhere, orderBy, (pageIndex - 1) * pageSize, pageSize);
        }
        /// <summary>
        /// 分页语句(联表查询)
        /// </summary>
        /// <param name="sql">传入的联表查询语句</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql(string sql, int pageIndex, int pageSize, string orderBy)
        {
            return string.Format("{0} ORDER BY {1} LIMIT {2},{3}", sql, orderBy, (pageIndex - 1) * pageSize, pageSize);
        }
    }
}
