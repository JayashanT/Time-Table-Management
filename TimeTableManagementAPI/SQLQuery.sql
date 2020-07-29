USE TimeTableDB

CREATE TABLE [Role] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC),
);

INSERT INTO Role VALUES('Admin')
INSERT INTO Role VALUES('Teacher')

CREATE TABLE [Users] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    [Staff_Id] NVARCHAR (MAX) NOT NULL UNIQUE,
    [Contact_No] NVARCHAR (MAX) NULL,
    [Password] NVARCHAR (MAX) NOT NULL,
	[Role_Id] INT NOT NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_User_Role] FOREIGN KEY ([Role_Id]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
);

ALTER TABLE [Users] DROP CONSTRAINT [UQ__Users__32D1F422D210ED03];
ALTER TABLE Users
ALTER COLUMN Password datatype;

ALTER TABLE Users
ADD UNIQUE (Staff_Id);

ALTER TABLE Users 
ALTER COLUMN Staff_Id NVARCHAR(450) ;

ALTER TABLE Users 
add Connection_Id NVARCHAR(max) ;

select * from users

INSERT INTO Users VALUES('Jayashan',0001,'0712043390','123',2) 
INSERT INTO Users VALUES('Thivanka',0002,'0766454594','0123',1)
INSERT INTO Users VALUES('EnglishTeacher',0003,'0766454594','123',2) 
INSERT INTO Users VALUES('EnglishTeacher-2',0004,'0766454594','123',2) 
INSERT INTO Users VALUES('EnglishTeacher-3',0005,'0766454594','123',2) 

CREATE TABLE [Attendance](
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[DATE] DATE NOT NULL,
	[STATUS] BIT NOT NULL,
	[User_Id] INT NOT NULL,
	CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Attendance_User] FOREIGN KEY ([User_Id]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
);

INSERT INTO Attendance VALUES(GETDATE(),1,4)
ALTER TABLE Attendance 
ADD CONSTRAINT [DF_Attendance_Status] DEFAULT 0 FOR STATUS

CREATE TABLE [Class] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    [Grade] INT NOT NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Id] ASC),
);

INSERT INTO Class output inserted.Id VALUES('5-C',5) 
INSERT INTO Class VALUES('2-A',2)
INSERT INTO Class VALUES('1-B',2)
INSERT INTO Class VALUES('2-B',2)
INSERT INTO Class VALUES('8-A',8)
INSERT INTO Class VALUES('8-B',8)

CREATE TABLE [Time_Table] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    [Grade] INT NOT NULL,
	[Class_Id] INT NOT NULL,
	[Admin_Id] INT NOT NULL,
    CONSTRAINT [PK_TimeTable] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_TimeTable_ClassId] FOREIGN KEY ([Class_Id]) REFERENCES [Class] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_TimeTable_Admin_Id] FOREIGN KEY ([Admin_Id]) REFERENCES [Users] ([Id]) 
);

INSERT INTO Time_Table VALUES('TIME TABLE 2020',1,1,2) 
INSERT INTO Time_Table VALUES('TIME TABLE 2020 2-A',2,2,2) 

CREATE TABLE [Subject] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
	[Medium] NVARCHAR (7) NULL,
    CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED ([Id] ASC),
);

INSERT INTO Subject VALUES('Maths','English') 
INSERT INTO Subject VALUES('Science','English') 
INSERT INTO Subject VALUES('English','English') 
INSERT INTO Subject VALUES('Sinhala','Sinhala') 


CREATE TABLE [Resource] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
	[Type] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Resource] PRIMARY KEY CLUSTERED ([Id] ASC),
);

INSERT INTO Resource VALUES('Sience lab 1','Academic') 
INSERT INTO Resource VALUES('Computer lab 1','Academic') 
INSERT INTO Resource VALUES('BasketBall','Sport') 
INSERT INTO Resource VALUES('Default','Default') 

SET IDENTITY_INSERT Resource ON
GO
INSERT INTO Resource VALUES(0,'Default','Default') 
GO
SET IDENTITY_INSERT Resource OFF

CREATE TABLE [Slot] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Day] NVARCHAR (MAX) NULL,
    [Period_No] INT NOT NULL,
	[Start_Time] DATETIME2 NOT NULL,
	[End_Time] DATETIME2 (4)  NOT NULL,
	[Time_Table_Id] INT NOT NULL,
	[Subject_Id] INT NOT NULL,
	[Resource_Id] INT NULL,
	[Teacher_Id] INT NULL,
    CONSTRAINT [PK_Slot] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Slot_Time_Table_Id] FOREIGN KEY ([Time_Table_Id]) REFERENCES [Time_Table] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Slot_Subject_Id] FOREIGN KEY ([Subject_Id]) REFERENCES [Subject] ([Id]) ,
	CONSTRAINT [FK_Slot_Resource_Id] FOREIGN KEY ([Resource_Id]) REFERENCES [Resource] ([Id]),
	CONSTRAINT [FK_Slot_Teacher_Id] FOREIGN KEY ([Teacher_Id]) REFERENCES [Users] ([Id]) 
);

ALTER TABLE Slot 
ALTER COLUMN Resource_Id DE;

ALTER TABLE Slot
ALTER COLUMN Period_NO NVARCHAR (MAX);

INSERT INTO Slot VALUES('Monday',1,'08:00','08:45',1,2,11,3) 
INSERT INTO Slot VALUES('Monday',2,'08:45','09:30',1,2,null,3)
INSERT INTO Slot VALUES('Monday',3,'09:30','10:15',1,2,null,3) 
INSERT INTO Slot VALUES('Monday',4,'09:30','10:15',1,3,null,4) 
INSERT INTO Slot VALUES('TUESDAY',4,'09:30','10:15',1,3,null,4) 
INSERT INTO Slot VALUES('Monday',2,'08:45','09:30',2,2,null,4)
INSERT INTO Slot VALUES('Monday',2,'08:45','09:30',2,2,null,3)
INSERT INTO Slot VALUES('Monday',3,'08:45','09:30',2,2,null,6)

CREATE TABLE [Updates] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Time] DATETIME2 NULL,
	[Status] NVARCHAR (MAX) NULL,
	[Slot_Id] INT NOT NULL,
	[Admin_Id] INT NOT NULL,
	[Teacher_Id] INT NOT NULL,
	[Period_No] INT NOT NULL,
    CONSTRAINT [PK_Changes] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Changes_Slot_Id] FOREIGN KEY ([Slot_Id]) REFERENCES [Slot] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Changes_Admin_Id] FOREIGN KEY ([Admin_Id]) REFERENCES [Users] ([Id]),
	CONSTRAINT [FK_Changes_Teacher_Id] FOREIGN KEY ([Teacher_Id]) REFERENCES [Users] ([Id])
);

INSERT INTO Changes VALUES (GETDATE(),'NOT ACCEPT',8,3,4)
ALTER TABLE Updates 
add Status bit

update Updates set Status=0 where id=2

CREATE TABLE [Teacher_Subject] (
    [Teacher_Id] INT NOT NULL,
	[Subject_Id] INT NOT NULL,
    CONSTRAINT [PK_Teacher_Subject] PRIMARY KEY CLUSTERED ([Teacher_Id],[Subject_Id]),
	CONSTRAINT [FK_Teacher_Subject_Teacher_Id] FOREIGN KEY ([Teacher_Id]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Teacher_Subject_Subject_Id] FOREIGN KEY ([Subject_Id]) REFERENCES [Subject] ([Id]) ON DELETE CASCADE,
);

INSERT INTO Teacher_Subject VALUES(2,1)
INSERT INTO Teacher_Subject VALUES(2,2)
INSERT INTO Teacher_Subject VALUES(4,3)
INSERT INTO Teacher_Subject VALUES(4,2)
INSERT INTO Teacher_Subject VALUES(6,2)


/*id=3 admin
id=2,4 teachers
*/

SELECT * FROM Role
SELECT * FROM Users
SELECT * FROM Class
SELECT * FROM Time_Table
SELECT * FROM Subject
SELECT * FROM Resource
SELECT * FROM Slot
SELECT * FROM Teacher_Subject
SELECT * FROM Updates
SELECT * FROM Attendance

delete  from subject where Id=5



/*all teachers allocated to a perticular time table*/
SELECT DISTINCT U.Name
FROM Users U
INNER JOIN Slot S
ON U.Id=S.Teacher_Id
WHERE S.Time_Table_Id=1



/*All resources allocated for a day*/
Select s.Period_No,R.Name,R.Id
FROM Resource R
INNER JOIN Slot S
ON S.Resource_Id=R.Id
WHERE S.Day LIKE 'Monday' AND S.Resource_Id NOT LIKE 'NULL'

/*All resources NOT allocated for a day*/
Select *
FROM Resource R
INNER JOIN Slot S
ON S.Resource_Id!=R.Id
WHERE S.Day LIKE 'Monday'

/*All resources NOT allocated for A perticular period in a day*/
Select R.Id, R.Name
FROM Resource R
LEFT JOIN Slot S
ON S.Resource_Id!=R.Id
WHERE S.Day LIKE 'Monday' AND S.Period_No=1

/*Get All teachers in school*/
SELECT Users.Name
from Users
INNER JOIN Role
ON Users.Role_Id=Role.Id
WHERE Role.Name LIKE 'Teacher'

/*Get All ADMINS in school*/
SELECT Users.Name
from Users
INNER JOIN Role
ON Users.Role_Id=Role.Id
WHERE Role.Name LIKE 'admin'

/*Time tables created by admin 1*/
SELECT *
FROM Time_Table
WHERE Admin_Id=3

/*Slots related to a time table*/
SELECT *
FROM Slot
WHERE Time_Table_Id=1

/*ALL SUBJECTS TEACHES BY A SINGLE TEACHER*/
SELECT *
FROM Subject
INNER JOIN Teacher_Subject
ON Subject.Id=Teacher_Subject.Subject_Id
WHERE Teacher_Id=4

/*All pending Changes by a teacher*/
SELECT *
FROM Changes
WHERE Status LIKE  'NOT ACCEPT'

/*get all periods by a teacher in a day*/
select *
from Slot
where Teacher_Id=1 AND day like 'Monday'

/* ISSUE UNSLOVED- unable to remove records with same day and period*/
/*All teachers having free time for perticular subject on perticular day for perticular period to assign a releif*/
Select*
FROM Users U
INNER JOIN Slot S
ON S.Teacher_Id=U.Id
INNER JOIN Teacher_Subject T
ON U.Id=T.Teacher_Id
WHERE U.Role_Id=2 AND U.Id!=2 AND T.Subject_Id=2 


/*All teachers having free time for perticular day for a perticular period to assign a releif*/
Select DISTINCT*
FROM Users U
INNER JOIN Slot S
ON S.Teacher_Id=U.Id
INNER JOIN Teacher_Subject T
ON U.Id=T.Teacher_Id
WHERE U.Role_Id=2 AND U.Id!=2
	
select distinct*
from users u
left join slot s
on u.Id=s.Teacher_Id
left join Teacher_Subject t
on u.Id=t.Teacher_Id
WHERE t.Subject_Id=2 AND u.Role_Id!=1

select * from Teacher_Subject where Subject_Id=2

select * from users u inner join slot s on u.Id = s.Teacher_Id left join Teacher_Subject t on u.Id = t.Teacher_Id WHERE T.Subject_Id = 1
select s.Period_No,u.name,u.id from users u left join slot s on u.Id = s.Teacher_Id left join Teacher_Subject t on u.Id = t.Teacher_Id WHERE t.Subject_Id = 1

select * from users
select * from Subject
s

select * from slot where id=6
Select * from Slot where Time_Table_Id=1

SELECT  s.Id,s.Day,s.Period_No,s.Time_Table_Id,s.Resource_Id,s.Teacher_Id,s.Subject_Id,sb.Name as S_Name
                FROM Slot s INNER JOIN Subject sb ON s.Subject_Id=sb.Id INNER JOIN users u ON s.Teacher_Id=u.Id WHERE s.Time_Table_Id=1

SELECT * FROM SLOT S INNER JOIN Subject SB ON S.Subject_Id=SB.Id WHERE s.Time_Table_Id=1

SELECT S.*, sb.Name as Subject_Name,C.Name AS Class_Name from Slot S  INNER JOIN Time_Table T ON S.Time_Table_Id=T.Id INNER JOIN Subject sb ON S.Subject_Id=sb.Id 
INNER JOIN Class C ON T.Class_Id=C.Id WHERE S.Teacher_Id=2

SELECT distinct S.*,SB.Name as Subject_Name FROM SLOT S INNER JOIN ATTENDANCE A ON S.Teacher_Id=A.User_Id INNER JOIN Subject SB on SB.Id=S.Subject_Id
WHERE A.Status=0 and a.DATE=CONVERT(date,GETDATE())

SELECT DAY('2017/08/25')

select * from updates

SELECT * from Updates where Date= CONVERT(date, GETDATE())