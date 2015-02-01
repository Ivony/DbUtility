DataPithy
=========



DataPithy is a light database access tool 


## How to use

execute a query
```CSharp
db.T( "SELECT * FROM Members" ).ExecuteDataTable();
```

execute a query with parameter and return first row
```CSharp
db.T( "SELECT FirstName, LastName FROM Members WHERE Username = {0}", username ).ExecuteFirstRow();
```

execute a query as async.
```CSharp
await db.T( "SELECT FirstName, LastName FROM Members WHERE Username = {0}", username ).ExecuteFirstRowAsync();
```

## Basic concept

```CSharp
db.T( "SELECT FirstName, LastName FROM Members WHERE Username = {0}", username ).ExecuteFirstRow();
```

In the code above, 
```db``` is called **database executor**,<br />
```T( "SELECT FirstName, LastName FROM Members WHERE Username = {0}", username )``` is called **query definition**,<br />
and ```ExecuteFirstRow()``` is called **result definition** <br />

---

The following code creates a **database executor**         
```CSharp
var db = SqlServer.Create( "connection-string-name" );
```
Or
```CSharp
var db = SqlServer.Connect( "connection-string" );
```

**database executor** is responsible for createing connection and executing queries.

----

A typical **query definition** like this below
```CSharp
db.T( "query-text-template", params parameters );
```
The query text is SQL command to be executed. and you can use parameter placehold inside like ```string.Format``` syntax. like this below:
```CSharp
db.T( "SELECT MemberID FROM Members WHERE Username = {0} AND Password = {1}", username, password )
```
it will create a SQL query like this below:
```SQL
DECLARE @Param0 AS nvarchar = 'text of username';
DECLARE @Param1 AS nvarchar = 'text of password';
SELECT MemberID FROM Members WHERE Username = @Param0 AND Password = @Param1;
```
the method name **T** means **Template**, so we can also write code like below:
```CSharp
db.Template( "SELECT MemberID FROM Members WHERE Username = {0} AND Password = {1}", username, password )
```
and the **T** is an **extension method**, you can declare another query definition method **as you like**.

---

In the last, we talk about the **result definition**.
like same as the query definition, result definition are also en extension method. we have many result definition method, and all of they have asynchronous version.
the popular result definition method under this:

**ExecuteNonQuery**, execute query, and return the number of rows affected.<br />
**ExecuteScaler**, execute query and return the first column of the first row.<br />
**ExecuteDataTable**, execute query and fill a DataTable and return.<br />
**ExecuteFirstRow**, execute query and return ths first row.<br />
**ExecuteEntity**, execute query and return the first row to fill the specified type of entity

## Get It

you can download last stable release from nuget:
[DataPithy](http://www.nuget.org/packages/DbWrench/)
