# MyDapper.ORM
基于Dapper的ORM框架，使用反射中的Emit技术，添加扩展IDataReader方法，提高转换数据实体性能。

1、MyDapper.ORM下的Core文件：基于Dapper扩展ORM框架的核心

2、MyDapper.ORM下的Dapper文件：Dapper源码，来自于第三方开源

3、MyDapper.Test实现了淫的简单三层
（1）-Dao：数据访问层
（2）-Service：业务访问层
（3）-Model：实体对象层

4、如需要切换数据库方式，可在BaseDao中进行配置，实际项目可将此配置放到配置文件中。