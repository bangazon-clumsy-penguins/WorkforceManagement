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

### Computers Model

### Computers Controller

### Computers View

## Departments

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

### Trainings Controller

### Trainings View

