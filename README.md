# AdventureWorksCore

This is a rework of the infant AdventureWorks project (https://github.com/jzdrake440/AdventureWorks) for the ASP.NET Core MVC framework.  The migration was mostly smooth and I have started working on bug fixes and new features.

This project is developing usability pages for the data definition for Adventure Works Cycles, a sample database provided by Microsoft.  As a recently discovered problem, the data definition has key issues that are now causing problems.  After some experimental development, the project has moved to a 3 tier architecture. This will promote better DRY principles.


## Problem 1
The DataTables api for serverSide requests will send data for which columns it's expecting, filter criteria and ordering criteria to the server and the server responds with the data.  This presents a challenge when working with linq to entities since dynamically referencing the properties in normal lambda expressions isn't possible.

### Solution 1
Build the lambda expressions dynamically using tools in the Linq.Expressions namespace.  This solution has been implemented, but still requires optimization for properties that don't map directly to columns (see problem 3).

## Problem 2
The first page being worked on is a Customers page.  This page is driven by the DataTables jquery plugin using serverside API.  The data definition described customers as being either an Individual or a Store and claimed that the sales.customer table has a "CustomerType" field denoting each with an "I" or "S".  This field doesn't actually exist, but there is a StoreId and PersonId field that appeared to be null or populated mutually exclusively.  As it turns out, this isn't true either.  There is 635 rows that contain both a StoreID and PersonID.  A third type of customer isn't referenced by the data definition, so I'll need to figure out what to do with this and redesign the customer data table page.

### Solution 2
After finding and inspecting the Sales.vIndividualCustomers view (db), there is a where condition for "StoreID is null".  So in future cases of determining the Customer Type, a check for Individuals (PersonID is not null) must come before the check for Store (StoreID is not null).

## Problem 3
Some columns I deemed necessary for the Customers DataTable do not directly correlate to properties in the entity model.  The dynamic expression builder expects the requested properties to exist in the entity model.

### Solution 3.1
Add a property to the entity model that returns the needed data.
#### Problem 3.1.1
This property wouldn't map to any db fields and linq queries would have to run locally.
####Solution 3.1.1
Add a custom property attribute that points at a different method that returns a constructed expression in place of the property expression.

### Solution 3.2
With the introduction of 3 tier architecture, I wanted to keep as few changes to the entity model as possible.  This meant that I could no longer use an unmapped property/attribute used in the first solution.
The new solution involved creating an injected service that maintains a Dictionary of custom properties.  The DataTablesService (previously a utility) would try to get the PropertyInfo for the requested data and upon failing to do so will look in the PpeService (Phantom Property Expression Service) for a mapping to a custom expression.