# RoomMates

Exercise in ADO.NET. 

- Your task is to build a command line application to manage a house full of roommates. You should persist data in a SQL Server database.

## Data Persistence

Our .NET application will need to communicate with a SQL Server database. To be able to do this, we'll be installing a library called ADO.NET. With ADO.NET installed, we'll have access to a few C# classes that are helpful for communicating with databases. The ADO.NET classes we'll be using heavily are:

- **SqlConnection** - This class represents the connection between our console application and our SQL Server database
- **SqlCommand** - This class will help us write sql queries in our C# code and execute them against the database
- **SqlDataReader** - This class will help us parse out the data that comes back from our database so that we can convert it to C# objects

## Vocabulary

Before you get started, let's introduce some terms that will be used during this project.

- **Models** - Models are C# classes that represent our database tables. For example, we have a `Chore` table in our database with a `Id` and `Name` column. To model this, we'd make a C# class named `Chore` with an `Id` and `Name` property.
- **Repository** - Repositories are classes that we create whose purpose is data access. We'll define lots of our CRUD functionality there. They often have methods like `Get`, `GetById`, `Add`, `Delete`, etc
- **Connection String** - A connection string is an address of a database--similar to a URL. It specifies the source of the data as well as the means of connecting to it. For example, you're about to create a database inside of SQL Server called `Roommates`; the connection string for the `Roommates` database will be `server=localhost\SQLExpress;database=Roommates;integrated security=true`
- **ADO.NET** - ADO.NET is an umbrella term for all of the C# classes we'll be using (listed above) for accessing our SQL database from our C# console app. 

-- CLI that teaches the basics of CRUD and connection to the SQL Server.  
