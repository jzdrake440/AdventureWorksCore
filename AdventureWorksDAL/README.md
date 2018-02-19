## Scaffolding
Scaffold-DbContext "Server=.;Database=AdventureWorks2017;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir DAL -Verbose -Force

Scaffolding could be done dynamically pre-build, but that leads to several potential consequences when this database will not be changing.
Do not use -DataAnnotations because these do not fully support many-to-many keys.