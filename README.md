DbUtility
=========

DbUtility is a light database access tool 


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
```db``` is called **database executor**,
```T( "SELECT FirstName, LastName FROM Members WHERE Username = {0}", username )``` is called **query define**,
and ```ExecuteFirstRow()``` is called **result define** 

---

The following code creates a **database executor**
```CSharp
var db = SqlDbUtility.Create( "connection-string-name" );
```
Or
```CSharp
var db = new SqlDbUtility( "connection-string" );
```

**database executor** is responsible for createing connection and executing queries.

----

A typical **query define** like this below
```CSharp
db.T( "query-text-template", params parameters );
```
The query text is SQL command to be executed. and you can use parameter placehold inside like ```string.Format``` syntax.

like this
```CSharp
db.T( "SELECT MemberID FROM Members WHERE Username = {0} AND Password = {1}", username, password )
```
it will create a SQL query like this below:
```SQL
DEFINE @Param0 as nvarchar = 'text of username';
DEFINE @Param1 as nvarchar = 'text of password';
SELECT MemberID FROM Members WHERE Username = @Param0 AND Password = @Param1;
```