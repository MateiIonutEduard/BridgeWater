## Description

This E-Commerce web type application, is used to shop various products for agriculture.<br/>
Application is built by using ASP.NET Core, with PostgreSQL and MongoDB as database systems.<br/>

## Required Steps
First, you need to install [<b>MongoDB</b>](https://www.mongodb.com/try/download/community) and [<b>PostgreSQL</b>](https://www.postgresql.org/download/) database servers.<br>
In the case of the <b>MongoDB</b>, create the database and name the main collection.<br/>
Finally, import <b>JSON</b> payload containing the collection scheme, now everything is ready.<br/><br/>
Next, create the <b>SQL</b> database in <b>pgAdmin</b>, with the name found in the configuration file.<br/>
After that, create the database from the included migration scheme, open Nuget Package Manager.<br><br><br>
<b>Package Manager Console</b>
```powershell
PM> Update-Database
PM> Remove-Migration
```
<br/>
<b>If you prefers terminal:</b>
<br/><br/>

```shell

> dotnet ef database update
> dotnet ef migrations remove

```
