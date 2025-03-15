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
