# AdventureWorksCore

This is a rework of the infant AdcentureWorks project (https://github.com/jzdrake440/AdventureWorks) for the ASP.NET Core MVC framework.  The migration was mostly smooth and I have started working on bug fixes and new features.

This project is developing usability pages for the data definition for Adventure Works Cycles, a sample database provided by Microsoft.  As a recently discovered problem, the data definition has key issues that are now causing problems.


## Problem 1
The DataTables api for serverSide requests will send data for which columns it's expecting, filter criteria and ordering criteria to the server and the server responds with the data.  This presents a challenge when working with linq to entities since dynamically referencing the properties in lambda expressions isn't possible without building dynamic expressions using the Linq.Expressions classes.  While this has mostly been achieved, optimization is still required as not all queries are running on db side.

## Problem 2
The first page being worked on is a Customers page.  This page is driven by the DataTables jquery plugin using serverside API.  The data definition described customers as being either an Individual or a Store and claimed that the sales.customer table has a "CustomerType" field denoting each with an "I" or "S".  This field doesn't actually exist, but there is a StoreId and PersonId field that appeared to be null or populated mutually exclusivly.  As it turns out, this isn't true either.  There is 635 rows that contain both a StoreID and PersonID.  A third type of customer isn't referenced by the data definition, so I'll need to figure out when to do with this and redesign the customer data table page.
