# WorkforceManagement

# The Bangazon Platform API (BangazonAPI)

Repo for first Bangazon sprint.

## Table of Contents

0. [Example](#example)
1. [Computers](#computers)
1. [Departments](#departments)
1. [Employees](#employees)
1. [Trainings](#trainings)

## Example

### Example Model

- Describe the Example model here

```c#
public class Example
{
	public int Id { get; set; }
	public string Whatever { get; set; }

	...etc
}
```

- If you needed to create any custom ViewModels, document those here also

```c#
[More code]
```

### Example Controller

Index

- Describe the Example controller Index( ) method here

Create

- [HttpGet] Describe the GET version of Create( ) here
- [HttpPost] Describe the POST version of Create( ) here

...etc

### Example View

[Have a screenshot of the view here maybe?]

- Describe any important things that someone might need to know about the view (what happens when you click stuff, etc.)

- Save this section for last

## Computers

From the home page `localhost:****` a user can click on the `Computers` link in the Navigation bar. This link will redirect the user to the Computers page where a list of all computers will be displayed.

When viewing the list of computers, a user can choose to "View" details for a given computer or "Create" a new computer by clicking the associated link. Users can also view a computers details by clicking on the on the individual Computer.

When the "Create New" link is clicked, the user will be taken to a form where they can enter the Manufacturer, Model, and PurchaseDate of the new computer. All fields are required. When a computer is successfully added to the database, the user will be redirected to the main computers view showing the list of all computers.

When a user is viewing the details of a single computer, they can click on the "Delete" link to delete a computer. If the computer has already been assigned to an employee, the user will be redirected to a page informing them that the computer cannot be deleted. If the computer has not been assigned to any employees, the user will be taken to a page asking them to confirm their choice of deleting the computer. Clicking the "Delete" button will remove the computer from the database and take the user back to the list of all computers. Clicking the "Cancel" button will take the user back to the list of all computers without deleting the computer from the database.

### Computers Model

### Computers Controller

### Computers View

## Departments

From the home page `localhost:****` a user can click on the `Departments` link in the Navigation bar. This link will redirect you to the Departments page where a list of all Departments will be displayed.

When the "Create New" link is clicked a user will be redirected to a form requesting a Name and Budget be supplied for the new department.

User can provide letters and numbers for the Name field but only numbers for the Budget field. If letters are provided in the Budget field an error message will be displayed.

A user cannot create a new department if that department name already exists. Currently the user will stay on the Create New department page if the department exists.

### Departments Model

```c#
public class Department
{
	public int Id { get; set; }

	[Required]
	[Display(Name="Department")]
	public string Name { get; set; }

	[Required]
	[Range(0.0, Double.PositiveInfinity)]
	public double Budget { get; set; }

	public List<Employee> EmployeeList { get; set; }
}
```

### Departments Controller

Index ( )

- [HttpGet] Gets a list of all the departments and then passes them to the index view

### Departments View

Index

- Shows a list of all the department names. Each name is a hyperlink that can be clicked to view the details of that department.

## Employees

### Employees Model

### Employees Controller

### Employees View

## Trainings

### Trainings Model

```c#
public class TrainingProgram : IValidatableObject
{
	public int Id { get; set; }

	[Required]
	[Display(Name = "Program")]
	public string Name { get; set; }

	public string Description { get; set; }

	[Required]
	[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
	[Display(Name = "Start Date")]
	public DateTime StartDate { get; set; }

	[Required]
	[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
	[Display(Name = "End Date")]
	public DateTime EndDate { get; set; }

	[Required]
	[Range(1, int.MaxValue, ErrorMessage = "Occupancy must be greater than 0!")]
	[Display(Name = "Max Occupancy")]
	public int MaxOccupancy { get; set; }

	[Display(Name = "Assigned Employees")]
	public List<Employee> AssignedEmployees { get; set; } = new List<Employee>();
}
```

### Trainings Controller

Index ( )

- [HttpGet] Gets a list of all the trainings and then passes them to the index view

Details ( )

- [HttpGet] Gets the details of an individual component and passes them to the detail view

Create ( )

- [HttpPost] Validates Create Form input data and posts new training to database

### Trainings View

Index

- Shows a list of all the training names. Each name is a hyperlink that can be clicked to view the details of that department.

Details

- Shows the details of a training program including start date, end date, and max occupancy, and current attendees.

Create

- Displays a form of free input fields and date selector fields with validation rules to enforce selection on required fields.
