-- Tạo cơ sở dữ liệu
CREATE DATABASE PRN212;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE PRN212;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Citizen', 'AreaLeader', 'Police')) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL
);

-- Tạo bảng Households
CREATE TABLE Households (
    HouseholdID INT PRIMARY KEY IDENTITY(1,1),
    HeadOfHouseholdID INT,
    Address NVARCHAR(MAX) NOT NULL,
    CreatedDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (HeadOfHouseholdID) REFERENCES Users(UserID)
);

-- Tạo bảng RegistrationDetail
CREATE TABLE RegistrationDetail (
    RegistrationDetailId INT PRIMARY KEY IDENTITY(1,1),
    Description NVARCHAR(MAX) NOT NULL,
    VerifyingIdentity NVARCHAR(MAX) NULL,
    VerifyingResidence NVARCHAR(MAX) NULL
);

-- Tạo bảng Registrations
CREATE TABLE Registrations (
    RegistrationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    RegistrationType NVARCHAR(50) CHECK (RegistrationType IN ('Permanent', 'Temporary', 'TemporaryStay')) NOT NULL,
    RegistrationDetailId INT REFERENCES RegistrationDetail(RegistrationDetailId),    
    StartDate DATE DEFAULT GETDATE(),
    EndDate DATE NULL,
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    ApprovedBy INT,
    Comments NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID)
);

-- Tạo bảng HouseholdMembers
CREATE TABLE HouseholdMembers (
    MemberID INT PRIMARY KEY IDENTITY(1,1),
    HouseholdID INT,
    UserID INT,
    Relationship NVARCHAR(50) NOT NULL,
    FOREIGN KEY (HouseholdID) REFERENCES Households(HouseholdID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng Notifications
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng Logs
CREATE TABLE Logs (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Action NVARCHAR(100) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Chèn dữ liệu mẫu vào bảng Users
INSERT INTO Users (FullName, Email, Password, Role, Address) VALUES
('Nguyen Van A', 'nguyenvana@example.com', 'hashed_password', 'Citizen', '123 Nguyen Trai'),
('Tran Thi B', 'tranthib@example.com', 'hashed_password', 'AreaLeader', '456 Le Loi'),
('Le Van C', 'levanc@example.com', 'hashed_password', 'Police', '789 Tran Hung Dao'),
('Pham Van D', 'phamvand@example.com', 'hashed_password', 'Citizen', '101 Bach Mai'),
('Hoang Thi E', 'hoangthie@example.com', 'hashed_password', 'Citizen', '202 Kim Ma'),
('Nguyen Van F', 'nguyenvanf@example.com', 'hashed_password', 'Citizen', '303 Tran Phu'),
('Tran Thi G', 'tranthig@example.com', 'hashed_password', 'Police', '404 Ba Trieu'),
('Le Van H', 'levanh@example.com', 'hashed_password', 'AreaLeader', '505 Hoang Hoa Tham'),
('Pham Thi I', 'phamthii@example.com', 'hashed_password', 'Citizen', '606 Le Duan'),
('Hoang Van J', 'hoangvanj@example.com', 'hashed_password', 'Citizen', '707 Hai Ba Trung');

-- Chèn dữ liệu mẫu vào bảng Households
INSERT INTO Households (HeadOfHouseholdID, Address) VALUES
(1, '123 Nguyen Trai'),
(2, '456 Le Loi'),
(3, '789 Tran Hung Dao'),
(4, '101 Bach Mai'),
(5, '202 Kim Ma'),
(6, '303 Tran Phu'),
(7, '404 Ba Trieu'),
(8, '505 Hoang Hoa Tham'),
(9, '606 Le Duan'),
(10, '707 Hai Ba Trung');

-- Chèn dữ liệu mẫu vào bảng RegistrationDetail
INSERT INTO RegistrationDetail (Description, VerifyingIdentity, VerifyingResidence) VALUES
('Permanent registration', 'ID123', 'Household123'),
('Temporary registration', 'ID456', 'Household456'),
('TemporaryStay registration', 'ID789', 'Household789'),
('Permanent registration', 'ID101', 'Household101'),
('Temporary registration', 'ID202', 'Household202');

-- Chèn dữ liệu mẫu vào bảng Registrations
INSERT INTO Registrations (UserID, RegistrationType, RegistrationDetailId, Status, ApprovedBy, Comments) VALUES
(1, 'Permanent', 1, 'Approved', 2, 'Verified'),
(2, 'Temporary', 2, 'Pending', NULL, NULL),
(3, 'TemporaryStay', 3, 'Rejected', 1, 'Invalid documents'),
(4, 'Permanent', 4, 'Approved', 3, 'All good'),
(5, 'Temporary', 5, 'Pending', NULL, 'Waiting for approval');

-- Chèn dữ liệu mẫu vào bảng HouseholdMembers
INSERT INTO HouseholdMembers (HouseholdID, UserID, Relationship) VALUES
(1, 1, 'Head'),
(2, 2, 'Head'),
(3, 3, 'Head'),
(4, 4, 'Head'),
(5, 5, 'Head'),
(6, 6, 'Head'),
(7, 7, 'Head'),
(8, 8, 'Head'),
(9, 9, 'Head'),
(10, 10, 'Head');