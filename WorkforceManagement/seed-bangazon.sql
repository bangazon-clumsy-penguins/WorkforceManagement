INSERT INTO ProductTypes (Label) VALUES 
('Balls')
,('Yo-Yos')
,('Shoes')
,('Bikes')
,('Guns')

INSERT INTO Customers (FirstName, LastName, JoinDate, LastInteractionDate) VALUES 
('Tom', 'Smith', '2016-01-01', '2017-01-01')
,('Jill', 'Johnson', '2016-01-02', '2018-01-01')
,('Bill', 'Williams', '2016-01-03', '2018-01-02')
,('Sue', 'Erickson', '2016-01-04', '2018-01-03')
,('Emily', 'Haluska', '2016-01-05', '2018-01-04')

INSERT INTO PaymentTypes (Label) VALUES 
('Visa')
,('MasterCard')
,('AmericanExpress')

INSERT INTO Products (Title, Description, Quantity, Price, ProductTypeId, CustomerId) VALUES 
('Football', 'Sick Football', 7, 47.5, (SELECT Id FROM ProductTypes WHERE Label='Balls'), (SELECT Id FROM Customers WHERE FirstName='Tom' AND LastName='Smith'))
,('RazorYo', 'Sweet Yo Yo', 6, 44.98, (SELECT Id FROM ProductTypes WHERE Label='Yo-Yos'), (SELECT Id FROM Customers WHERE FirstName='Jill' AND LastName='Johnson'))
,('AirJordans', 'Really nice shoes', 5, 5000.48, (SELECT Id FROM ProductTypes WHERE Label='Shoes'), (SELECT Id FROM Customers WHERE FirstName='Bill' AND LastName='Williams'))
,('Huffy', 'Great bike bro', 4, 746.12, (SELECT Id FROM ProductTypes WHERE Label='Bikes'), (SELECT Id FROM Customers WHERE FirstName='Sue' AND LastName='Erickson'))
,('Colt 45', 'Deadly Weapon', 3, 1222.56, (SELECT Id FROM ProductTypes WHERE Label='Guns'), (SELECT Id FROM Customers WHERE FirstName='Emily' AND LastName='Haluska'))

INSERT INTO CustomerAccounts (AccountNumber, CustomerId, PaymentTypeId) VALUES 
(461898165, (SELECT Id FROM Customers WHERE FirstName='Tom' AND LastName='Smith'), (SELECT Id FROM PaymentTypes WHERE Label='Visa'))
,(498354815, (SELECT Id FROM Customers WHERE FirstName='Jill' AND LastName='Johnson'), (SELECT Id FROM PaymentTypes WHERE Label='MasterCard'))
,(645987456, (SELECT Id FROM Customers WHERE FirstName='Bill' AND LastName='Williams'), (SELECT Id FROM PaymentTypes WHERE Label='Visa'))
,(623145284, (SELECT Id FROM Customers WHERE FirstName='Sue' AND LastName='Erickson'), (SELECT Id FROM PaymentTypes WHERE Label='AmericanExpress'))
,(561862348, (SELECT Id FROM Customers WHERE FirstName='Emily' AND LastName='Haluska'), (SELECT Id FROM PaymentTypes WHERE Label='Visa'))

INSERT INTO Orders (CustomerId, CustomerAccountId) VALUES 
((SELECT Id FROM Customers WHERE FirstName='Tom' AND LastName='Smith'), (SELECT Id FROM CustomerAccounts WHERE AccountNumber=461898165))
,((SELECT Id FROM Customers WHERE FirstName='Jill' AND LastName='Johnson'), (SELECT Id FROM CustomerAccounts WHERE AccountNumber=498354815))
,((SELECT Id FROM Customers WHERE FirstName='Bill' AND LastName='Williams'), (SELECT Id FROM CustomerAccounts WHERE AccountNumber=645987456))
,((SELECT Id FROM Customers WHERE FirstName='Sue' AND LastName='Erickson'), (SELECT Id FROM CustomerAccounts WHERE AccountNumber=623145284))
,((SELECT Id FROM Customers WHERE FirstName='Emily' AND LastName='Haluska'), (SELECT Id FROM CustomerAccounts WHERE AccountNumber=561862348))

INSERT INTO OrderedProducts (ProductId, OrderId) VALUES 
((SELECT Id From Products Where Title='Football'), 1)
,((SELECT Id From Products Where Title='RazorYo'), 2)
,((SELECT Id From Products Where Title='AirJordans'), 3)
,((SELECT Id From Products Where Title='Huffy'), 4)
,((SELECT Id From Products Where Title='Colt 45'), 5)

INSERT INTO Computers (Manufacturer, Model, PurchaseDate, DecommissionDate) VALUES 
('PC', 'Awesome-O 3000', '2018-01-01', null)
,('Mac', 'Poop-top', '2018-01-02', null)
,('PC', 'The Bomb.com', '2018-01-02', null)
,('Mac', 'Lap-crap', '2018-01-02', null)
,('PC', 'Super-Compute 1000', '2018-01-02', null)

INSERT INTO Departments (Name, Budget) VALUES 
('Finance', 4000)
,('Maintenance', 5000)
,('HR', 6000)
,('Accounting', 7000)
,('Marketing', 80000)

INSERT INTO Trainings (Name, Description, StartDate, EndDate, MaxOccupancy) VALUES 
('Safety', 'Safety on the premises', '2018-05-05', '2018-06-09', 10)
,('Anti-Terrorism', 'Fending off bad guys', '2018-05-06', '2018-06-10', 10)
,('Write Better Code', 'Cause you should', '2018-05-07', '2018-06-11', 12)
,('Spelling Training', 'Cause you should', '2018-05-08', '2018-06-12', 12)
,('Physical Training', 'No pain no gain', '2018-05-09', '2018-06-13', 12)

INSERT INTO Employees (FirstName, LastName, HireDate, IsSupervisor, DepartmentId) VALUES 
('Tom', 'Smith', '2015-05-01', 1, (SELECT Id FROM Departments WHERE Name='Finance'))
,('Sarah', 'Johnson', '2015-05-02', 1, (SELECT Id FROM Departments WHERE Name='Maintenance'))
,('Bill', 'Williams', '2015-05-03', 1, (SELECT Id FROM Departments WHERE Name='HR'))
,('Sue', 'Erickson', '2015-05-04', 1, (SELECT Id FROM Departments WHERE Name='Accounting'))
,('Emily', 'Haluska', '2015-05-05', 1, (SELECT Id FROM Departments WHERE Name='Marketing'))

INSERT INTO EmployeeComputers (AssignmentDate, ReturnDate, EmployeeId, ComputerId) VALUES 
('2018-06-06', null, 1, 5)
,('2018-06-07', null, 2, 4)
,('2018-06-08', null, 3, 3)
,('2018-06-09', null, 4, 2)
,('2018-06-10', null, 5, 1)

INSERT INTO EmployeeTrainings (EmployeeId, TrainingId) VALUES 
(1, 2)
,(2, 3)
,(3, 1)
,(4, 5)
,(5, 4)

