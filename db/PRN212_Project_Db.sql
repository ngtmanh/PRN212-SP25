CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Citizen', 'AreaLeader', 'Police')) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Households (
    HouseholdID INT PRIMARY KEY IDENTITY(1,1),
    HeadOfHouseholdID INT,
    Address NVARCHAR(MAX) NOT NULL,
    CreatedDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (HeadOfHouseholdID) REFERENCES Users(UserID)
);

create table RegistrationDetail (
	RegistrationDetailId int primary key identity(1,1),
	Description NVARCHAR(max) NOT NULL,
	VerifyingIdentity nvarchar(max) null,
	VerifyingResidence nvarchar(max) null
)

CREATE TABLE Registrations (
    RegistrationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    RegistrationType NVARCHAR(50) CHECK (RegistrationType IN ('Permanent', 'Temporary', 'TemporaryStay')) NOT NULL,
    RegistrationDetailId int references RegistrationDetail(RegistrationDetailId),    
    StartDate DATE DEFAULT GETDATE(),
    EndDate DATE NULL,
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    ApprovedBy INT,
    Comments NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID)
);

CREATE TABLE HouseholdMembers (
    MemberID INT PRIMARY KEY IDENTITY(1,1),
    HouseholdID INT,
    UserID INT,
    Relationship NVARCHAR(50) NOT NULL,
    FOREIGN KEY (HouseholdID) REFERENCES Households(HouseholdID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Logs (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Action NVARCHAR(100) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Insert Users
INSERT INTO Users (FullName, Email, Password, Role, Address) VALUES
('Nguyen Van A', 'a@example.com', 'hashed_password1', 'Citizen', '123 Street, City'),
('Tran Thi B', 'b@example.com', 'hashed_password2', 'Citizen', '456 Street, City'),
('Le Van C', 'c@example.com', 'hashed_password3', 'AreaLeader', '789 Street, City'),
('Pham Thi D', 'd@example.com', 'hashed_password4', 'Police', '101 Street, City'),
('Hoang Van E', 'e@example.com', 'hashed_password5', 'Citizen', '202 Street, City'),
('Nguyen Thi F', 'f@example.com', 'hashed_password6', 'Citizen', '303 Street, City'),
('Tran Van G', 'g@example.com', 'hashed_password7', 'AreaLeader', '404 Street, City'),
('Le Thi H', 'h@example.com', 'hashed_password8', 'Police', '505 Street, City'),
('Pham Van I', 'i@example.com', 'hashed_password9', 'Citizen', '606 Street, City'),
('Hoang Thi J', 'j@example.com', 'hashed_password10', 'Citizen', '707 Street, City');

-- Insert Households
INSERT INTO Households (HeadOfHouseholdID, Address) VALUES
(1, '123 Street, City'),
(2, '456 Street, City'),
(5, '202 Street, City'),
(6, '303 Street, City'),
(9, '606 Street, City');

-- Insert RegistrationDetail
INSERT INTO RegistrationDetail (Description, VerifyingIdentity, VerifyingResidence) VALUES
('Permanent residence registration', 'ID12345', 'Household123'),
('Temporary residence registration', 'ID67890', 'Household456'),
('Temporary stay registration', 'ID54321', 'Household789'),
('Change of residence', 'ID98765', 'Household101'),
('New household registration', 'ID13579', 'Household202');

-- Insert Registrations
INSERT INTO Registrations (UserID, RegistrationType, RegistrationDetailId, StartDate, Status, ApprovedBy, Comments) VALUES
(1, 'Permanent', 1, '2024-01-01', 'Approved', 3, 'Verified'),
(2, 'Temporary', 2, '2024-02-01', 'Pending', NULL, NULL),
(3, 'TemporaryStay', 3, '2024-03-01', 'Rejected', 4, 'Missing documents'),
(4, 'Permanent', 4, '2024-04-01', 'Approved', 3, 'All clear'),
(5, 'TemporaryStay', 5, '2024-05-01', 'Pending', NULL, NULL);

-- Insert HouseholdMembers
INSERT INTO HouseholdMembers (HouseholdID, UserID, Relationship) VALUES
(1, 1, 'Head'),
(1, 2, 'Spouse'),
(2, 5, 'Head'),
(3, 6, 'Head'),
(3, 7, 'Child');

-- Insert Notifications
INSERT INTO Notifications (UserID, Message, SentDate, IsRead) VALUES
(1, 'Your registration has been approved.', '2025-03-23 10:00:00', 1),
(2, 'Your registration is pending.', '2025-03-23 10:05:00', 0),
(3, 'Your registration has been rejected.', '2025-03-23 10:10:00', 1),
(4, 'Your document verification is complete.', '2025-03-23 10:15:00', 0),
(5, 'Please update your registration details.', '2025-03-23 10:20:00', 0);

-- Insert Logs
INSERT INTO Logs (UserID, Action, Timestamp) VALUES
(1, 'Logged in', '2025-03-23 08:00:00'),
(2, 'Updated profile', '2025-03-23 08:30:00'),
(3, 'Submitted registration', '2025-03-23 09:00:00'),
(4, 'Approved registration', '2025-03-23 09:30:00'),
(5, 'Logged out', '2025-03-23 10:00:00');
