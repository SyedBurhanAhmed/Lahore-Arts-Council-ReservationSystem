use ReservationSystem;

Select * from bookings;
Select * from Facilities;
Select * from Bookings;

Drop table Admins;
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
	FatherName NVARCHAR(100),
    CNIC NVARCHAR(15),
	PhoneNumber VARCHAR(11),
	Email VARCHAR(100),
	Password VARCHAR(25)
);
/*
drop table users;
 */
 Select * from BOokings;
ALTER TABLE Users
	ADD Password VARCHAR(25);

	drop table admins;
CREATE TABLE Admins (
	AdminID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
	Email VARCHAR(100),
	Password VARCHAR(25)

);

drop table Admins;
INSERT INTO Admins Values('Burhan','sbahmed515@gmail.com','12');
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    FacilityID INT FOREIGN KEY REFERENCES Facilities(FacilityID),
    FirstBookingDate VARCHAR(11),
	LastBookingDate VARCHAR(11),
	TotalDays INT,
    StartTime VARCHAR(5),
    EndTime VARCHAR(5),
    ApplicationFormPath NVARCHAR(255),
	Topic NVARCHAR(50),
    IsBookingApproved INT DEFAULT 0,
	SecurityFeePath NVARCHAR(255),
	IsSecurityFeeApproved INT DEFAULT 0
);
EXEC sp_rename 'Bookings.IsApproved', 'IsBookingApproved', 'COLUMN';

Select * from Bookings;
ALTER TABLE Bookings
    ADD IsSecurityFeeApproved INT DEFAULT 0;


SELECT COUNT(*) 
FROM Bookings  
WHERE FacilityID = 1  
AND '20-10-2024' BETWEEN FirstBookingDate AND LastBookingDate  
AND ((StartTime < '10:30' AND EndTime > '08:00'))  
AND IsApproved = 1;


select * from Bookings;
Drop table bookings;
ALTER TABLE Bookings
ADD COLUMN ApplicationForm NVARCHAR(255),

Select * from Facilities;
Select * from Bookings;
EXEC sp_rename Bookings.'IsCancelled', 'IsApproved';

CREATE TABLE BannedTopics (
    TopicID INT PRIMARY KEY IDENTITY(1,1),
    Topic NVARCHAR(255)
);
drop view BookingDetails;
CREATE VIEW BookingDetails AS
SELECT 
    B.BookingID,
    U.Name AS UserName,
    U.CNIC,
    B.Topic,
    F.FacilityName,
	F.Complex AS ComplexName,
    B.FirstBookingDate,
	B.LastBookingDate,
    B.StartTime,
    B.EndTime,
	B.IsBookingApproved,
	B.SecurityFeePath,
	B.IsSecurityFeeApproved
FROM 
    Bookings B
JOIN 
    Users U ON B.UserID = U.UserID
JOIN 
    Facilities F ON B.FacilityID = F.FacilityID;


 UPDATE Bookings
 Set IsSecurityFeeApproved = 0
 Where IsBookingApproved = -1 or IsBookingApproved = 1 ;

  UPDATE Bookings
 Set IsBookingApproved = 0

 UPDATE Bookings 
 Set SecurityFeePath = NUll;

use ReservationSystem;
Select * from BookingDetails;
Select * from Users;
Drop view B
insert into table banned
Select * from Bookings;

drop table bookings;
