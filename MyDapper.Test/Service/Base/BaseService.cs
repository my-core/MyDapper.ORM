using MyDapper.ORM.Core;

using System.Configuration;
using System.Data;
using System;
using System.Collections.Generic;
using MyDapper.Test.Dao;

namespace MyDapper.Test.Service
{
    public class BaseService : IBaseService
    {
        public BaseDao baseDao;
        public BaseService()
        {
            baseDao = new BaseDao();
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
            return baseDao.Insert<T>(t);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t)
        {
            return baseDao.Update<T>(t);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Delete<T>()
        {
            return baseDao.Delete<T>();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public bool Delete<T, R>(R parms)
        {
            return baseDao.Delete<T,R>(parms);
        }

        /// <summary>
        /// 查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public T GetModel<T, R>(R parms)
        {
            return baseDao.GetModel<T, R>(parms);
        }

        /// <summary>
        /// 查询(多记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public List<T> GetList<T, R>(R parms)
        {
            return baseDao.GetList<T, R>(parms);
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
            return baseDao.GetPageList<T, R>(sql,parms,pageIndex,pageSize,orderBy);
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
            return baseDao.GetPageList<T>(pageIndex, pageSize, orderBy);
        }
        #endregion
    }
}
