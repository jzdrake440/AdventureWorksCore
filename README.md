# AdventureWorksCore

DISCONTINUED:  After finishing a few features and moving on to identity, I got frustrated with the inaccurate documentation.  While searching for updated documentation in github, I found that Microsoft has released a new(er) sample database called "World Wide Imports".  After looking at much better documentation, I have decided to move on to a new project based on that database.  Much of my code will come with me such as the serverSide api for jquery.dataTables.

This is a rework of the infant AdventureWorks project (https://github.com/jzdrake440/AdventureWorks) for the ASP.NET Core MVC framework.  The migration was mostly smooth and I have started working on bug fixes and new features.

This project is developing usability pages for the data definition for Adventure Works Cycles, a sample database provided by Microsoft.  As a recently discovered problem, the data definition has key issues that are now causing problems.  After some experimental development, the project has moved to a 3 tier architecture. This will promote better DRY principles.

## Completed Features
### Feature 1
Implemented Entity Framework Core with Database First approach.
### Feature 2
Implemented jquery.dataTables with serverSide processing.
### Feature 3
Created a dynamic expression builder for DataTables to allow dynamic queries through the entity framework.
### Feature 4
Create Phantom Property Expression Service to intercept custom properties that exist in the BLL, but not in the DAL.  The PPE Service would return an Expression to allow the custom property to execute on the db side.
### Feature 5
Created a mini-plugin for jquery.dataTables to allow column specific searching by adding a search input to the header of each column.
### Feature 6
Configured Extensions:
-Bundler and Minifier
-AutoMapper
### Feature 7
Created 2 tier navbar using Bootstrap 4


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