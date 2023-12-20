# Hajir
This is a .net core project to be used in conjunction with an implementation of MS Dynamic Crm in Hajir Sanat.
Hajir Sanat is an Iranian UPS Manufacturer.
The main purpose of this project is to add features that are not usually supported by MS Dynamics and it that sense it is considered as an extension.
It is supposed to provide follwing features:
* Product Management
* Sales


From the code base we will have following C# projects:
* Hajir.Crm: The main project that implements the rquired features.
* Hajir.Crm.Blazor: A blazor server component library that provides the razor components of the user interface.
* Hajir.Crm.Blazor.Server: A blazor server apllication that will host ui and api.
* Hajir.Crm.Infrastructure.Xrm: A MS Dynamic implementation of the required infrastructure. This is where we connect to the MS Dynamic.
* Hajir.Crm.Tests: Test project. We'll try to put tests and guidelines here, so that one can use these tests to figure out how the program works.

Note that the Library folder contains a set of helper projects that are imported from Gostareh Negar code base to be used here.







