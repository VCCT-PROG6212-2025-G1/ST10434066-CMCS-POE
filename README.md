
# Contract Monthly Claim System (CMCS)

**Author:** Fay-yaad Ahmed Parker 
**Module:** PROG6212 – POE

**Language:** C# (.NET 8 MVC)



## Overview

The Contract Monthly Claim System (CMCS) is a simple ASP.NET Core MVC web app that manages monthly lecturer claims.
It uses a role based workflow:
		Lecturer submits claims
		Coordinator verifies
		Manager approves or rejects
		HR manages users and generates reports

The system uses an InMemory database, Sessions, LINQ, and simple micro-APIs for validation and calculations.



  ## Key Features

		Session based login for all user roles
		Automated amount calculation 
		Secure file uploads 
		Separated workflows for Coordinator and Manager
		HR dashboard for user creation, password resets, and hourly rate updates
		Monthly reports grouped by lecturer
		Cleaner layout and improved navigation



## Lecturer Feedback 

Feedback: “Design could be improved.”
UI cleaned and navigation simplified

Feedback: “Coordinator and Manager must have separate views.”
Views split and correct workflow enforced

Feedback: “Large files crash the app.”
File size validation and error messages

Feedback: “Need more descriptive commits.”
Commit messages improved


## Seeded users

HR
	Username: HR
	Password: HR123

Manager
	Username: manager
	Password: manager123

Coordinator
	Username: coordinator
	Password: coord

Lecturer
	Username: Aisha
	Password: AishaParker



## YouTube Demonstration 

https://youtu.be/JBawAJTxlPg



## How to Run
	1.	Clone the repo
	2.	Open in Visual Studio
	3.	Run the app 
	4.	Login using the details above





