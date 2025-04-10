-- -- Tạo cơ sở dữ liệu
 -- Drop DATABASE UniversityDb;
-- GO
-- -- dotnet ef dbcontext scaffold "Server=(local);uid=sa;pwd=12345;database=UniversityDb;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models
-- -- Tạo cơ sở dữ liệu
-- USE UniversityDb;
-- GO

-- Tạo cơ sở dữ liệu
 CREATE DATABASE UniversityDb;
 GO
 USE UniversityDb;
 GO

-- Bảng vai trò người dùng
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleName VARCHAR(20) UNIQUE NOT NULL
);

-- Bảng phòng ban
CREATE TABLE Departments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DepartmentName VARCHAR(100) UNIQUE NOT NULL
);

-- Bảng người dùng
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Gender VARCHAR(10),
    Image VARCHAR(MAX),
    DateOfBirth DATE,
    PhoneNumber VARCHAR(20),
    Address NVARCHAR(255),
    RoleId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Bảng chuyên ngành
CREATE TABLE Majors (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MajorName NVARCHAR(100) NOT NULL,
    RequiredCredits INT NOT NULL,
    DepartmentID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng sinh viên
CREATE TABLE Students (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MSSV VARCHAR(100) NOT NULL,
    UserID UNIQUEIDENTIFIER NOT NULL,
    MajorID UNIQUEIDENTIFIER NOT NULL,
    EnrollmentDate DATE NOT NULL,
    AdmissionDate DATE,
    AdmissionStatus VARCHAR(20),
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (MajorID) REFERENCES Majors(Id)
);

-- Bảng giảng viên
CREATE TABLE Professors (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    DepartmentID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng nhân viên hành chính
CREATE TABLE AdministrativeStaff (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    DepartmentID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng chính sách học phí
CREATE TABLE TuitionPolicies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PolicyName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EffectiveDate DATE NOT NULL,
    ExpirationDate DATE,
    MajorID UNIQUEIDENTIFIER,
    FOREIGN KEY (MajorID) REFERENCES Majors(Id)
);

-- Bảng học bổng
CREATE TABLE Scholarships (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScholarshipName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EligibilityCriteria NVARCHAR(MAX),
    ApplicationDeadline DATE,
    DepartmentID UNIQUEIDENTIFIER,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng hỗ trợ tài chính
CREATE TABLE FinancialAids (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AidName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EligibilityCriteria NVARCHAR(MAX),
    ApplicationDeadline DATE,
    DepartmentID UNIQUEIDENTIFIER,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng liên kết sinh viên với học bổng
CREATE TABLE StudentScholarships (
    StudentID UNIQUEIDENTIFIER NOT NULL,
    ScholarshipID UNIQUEIDENTIFIER NOT NULL,
    AwardDate DATE NOT NULL,
    PRIMARY KEY (StudentID, ScholarshipID),
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (ScholarshipID) REFERENCES Scholarships(Id)
);

-- Bảng liên kết sinh viên với hỗ trợ tài chính
CREATE TABLE StudentFinancialAids (
    StudentID UNIQUEIDENTIFIER NOT NULL,
    FinancialAidID UNIQUEIDENTIFIER NOT NULL,
    AwardDate DATE NOT NULL,
    PRIMARY KEY (StudentID, FinancialAidID),
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (FinancialAidID) REFERENCES FinancialAids(Id)
);

-- Bảng phòng học
CREATE TABLE Classroom (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoomName NVARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    Location NVARCHAR(100),
    Status VARCHAR(20),
    Equipment NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng môn học
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseCode NVARCHAR(20) UNIQUE NOT NULL,
    CourseName NVARCHAR(100) NOT NULL,
    Credits INT NOT NULL,
    Description NVARCHAR(MAX),
    LearningOutcomes NVARCHAR(MAX),
    DepartmentID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng điều kiện tiên quyết
CREATE TABLE CoursePrerequisites (
    CourseID UNIQUEIDENTIFIER NOT NULL,
    PrerequisiteCourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (CourseID, PrerequisiteCourseID),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (PrerequisiteCourseID) REFERENCES Courses(Id)
);

-- Bảng điều kiện đồng tiên quyết
CREATE TABLE CourseCorequisites (
    CourseID UNIQUEIDENTIFIER NOT NULL,
    CorequisiteCourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (CourseID, CorequisiteCourseID),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (CorequisiteCourseID) REFERENCES Courses(Id)
);

-- Bảng đề cương môn học
CREATE TABLE CourseSyllabi (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseID UNIQUEIDENTIFIER NOT NULL,
    SyllabusContent NVARCHAR(MAX),
    Version NVARCHAR(50),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    FOREIGN KEY (CourseID) REFERENCES Courses(Id)
);

-- Bảng học kỳ
CREATE TABLE Semesters (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SemesterName VARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL
);

-- Bảng học phí sinh viên
CREATE TABLE StudentTuition (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    SemesterID UNIQUEIDENTIFIER NOT NULL,
    TuitionPolicyID UNIQUEIDENTIFIER NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    AmountPaid DECIMAL(18,2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18,2) DEFAULT 0,
    PaymentStatus NVARCHAR(20) NOT NULL,
    PaymentDueDate DATE NOT NULL,   
    PaymentDate DATETIME,   
    Notes NVARCHAR(MAX), 
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (SemesterID) REFERENCES Semesters(Id),
    FOREIGN KEY (TuitionPolicyID) REFERENCES TuitionPolicies(Id)
);

-- Bảng lớp học phần
CREATE TABLE ClassSections (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseID UNIQUEIDENTIFIER NOT NULL,
    SemesterID UNIQUEIDENTIFIER NOT NULL,
    ProfessorID UNIQUEIDENTIFIER,
    ClassroomID UNIQUEIDENTIFIER,
    Capacity INT NOT NULL,
    IsOnline BIT DEFAULT 0,
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (SemesterID) REFERENCES Semesters(Id),
    FOREIGN KEY (ProfessorID) REFERENCES Professors(Id),
    FOREIGN KEY (ClassroomID) REFERENCES Classroom(Id)
);

-- Bảng lịch học cho lớp học phần
CREATE TABLE ClassSectionSchedules (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClassSectionID UNIQUEIDENTIFIER NOT NULL,
    DayOfWeek VARCHAR(10) NOT NULL CHECK (DayOfWeek IN ('Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun')),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (ClassSectionID) REFERENCES ClassSections(Id)
);

-- Bảng đăng ký môn học
CREATE TABLE CourseRegistrations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    ClassSectionID UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate DATETIME NOT NULL,
    Status VARCHAR(20) NOT NULL,
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (ClassSectionID) REFERENCES ClassSections(Id)
);

-- Bảng điểm số
CREATE TABLE Grades (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseRegistrationID UNIQUEIDENTIFIER NOT NULL,
    GradeValue VARCHAR(5) NOT NULL CHECK (GradeValue IN ('A', 'B', 'C', 'D', 'F')),
    QualityPoints DECIMAL(5,2) NOT NULL,
    FOREIGN KEY (CourseRegistrationID) REFERENCES CourseRegistrations(Id)
);

-- Bảng đánh giá giữa kỳ
CREATE TABLE MidtermEvaluations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseRegistrationID UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Recommendation NVARCHAR(MAX),
    FOREIGN KEY (CourseRegistrationID) REFERENCES CourseRegistrations(Id)
);

-- Bảng yêu cầu dịch vụ
CREATE TABLE ServiceRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    ServiceType NVARCHAR(50) NOT NULL,
    RequestDate DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Details NVARCHAR(MAX),
    FOREIGN KEY (StudentID) REFERENCES Students(Id)
);

-- Bảng đánh giá môn học cuối kỳ
CREATE TABLE CourseEvaluations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClassSectionID UNIQUEIDENTIFIER NOT NULL,
    StudentID UNIQUEIDENTIFIER NOT NULL,
    Rating INT NOT NULL,
    Comments NVARCHAR(MAX),
    EvaluationDate DATETIME NOT NULL,
    FOREIGN KEY (ClassSectionID) REFERENCES ClassSections(Id),
    FOREIGN KEY (StudentID) REFERENCES Students(Id)
);

-- Bảng liên kết giữa chuyên ngành và môn học
CREATE TABLE MajorCourses (
    MajorID UNIQUEIDENTIFIER NOT NULL,
    CourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (MajorID, CourseID),
    FOREIGN KEY (MajorID) REFERENCES Majors(Id),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id)
);

INSERT INTO Roles (Id, RoleName) VALUES
('10000000-0000-0000-0000-000000000001', 'Admin'),
('10000000-0000-0000-0000-000000000002', 'Student'),
('10000000-0000-0000-0000-000000000003', 'Professor'),
('10000000-0000-0000-0000-000000000004', 'Staff');

INSERT INTO Departments (Id, DepartmentName) VALUES
('20000000-0000-0000-0000-000000000001', 'Phòng Đào tạo (PĐT)'),
('20000000-0000-0000-0000-000000000002', 'Phòng Kế toán'),
('20000000-0000-0000-0000-000000000003', 'Phòng Công tác Sinh viên (CTSV)'),
('30000000-0000-0000-0000-000000000001', 'Khoa Công nghệ Thông tin'),
('30000000-0000-0000-0000-000000000002', 'Khoa Quản trị Kinh doanh');

INSERT INTO Users (Id, Email, Password, FullName, Image, RoleId) VALUES
('40000000-0000-0000-0000-000000000001', 'admin@university.com', 'hashed_pass1', 'Admin User', NULL, '10000000-0000-0000-0000-000000000001'),
('40000000-0000-0000-0000-000000000002', 'student1@university.com', 'hashed_pass2', 'Nguyen Van A', NULL, '10000000-0000-0000-0000-000000000002'),
('40000000-0000-0000-0000-000000000003', 'professor1@university.com', 'hashed_pass3', 'Tran Thi B', NULL, '10000000-0000-0000-0000-000000000003'),
('40000000-0000-0000-0000-000000000004', 'staff_pdt@university.com', 'hashed_pass4', 'Le Van C', NULL, '10000000-0000-0000-0000-000000000004'),
('40000000-0000-0000-0000-000000000005', 'staff_ketoan@university.com', 'hashed_pass5', 'Pham Thi D', NULL, '10000000-0000-0000-0000-000000000004');

INSERT INTO Majors (Id, MajorName, RequiredCredits, DepartmentID) VALUES
('50000000-0000-0000-0000-000000000001', 'Cử nhân Công nghệ Thông tin', 120, '30000000-0000-0000-0000-000000000001'),
('50000000-0000-0000-0000-000000000002', 'Cử nhân Quản trị Kinh doanh', 130, '30000000-0000-0000-0000-000000000002');

INSERT INTO Students (Id, MSSV, UserID, MajorID, EnrollmentDate, AdmissionDate, AdmissionStatus) VALUES
('60000000-0000-0000-0000-000000000001', 'SV001', '40000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', '2023-09-01', '2023-08-01', 'Accepted');

INSERT INTO Professors (Id, UserID, DepartmentID) VALUES
('70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001');

INSERT INTO AdministrativeStaff (Id, UserID, DepartmentID) VALUES
('80000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000001'), -- PĐT
('80000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002'); -- Kế toán

INSERT INTO TuitionPolicies (Id, PolicyName, Description, Amount, EffectiveDate, ExpirationDate, MajorID) VALUES
('90000000-0000-0000-0000-000000000001', 'Học phí CNTT 2023', 'Học phí theo tín chỉ', 500000.00, '2023-01-01', NULL, '50000000-0000-0000-0000-000000000001');

INSERT INTO Scholarships (Id, ScholarshipName, Description, Amount, EligibilityCriteria, ApplicationDeadline, DepartmentID) VALUES
('A0000000-0000-0000-0000-000000000001', 'Học bổng Xuất sắc 2023', 'Dành cho SV top 10%', 5000000.00, 'GPA > 3.5', '2023-07-15', '20000000-0000-0000-0000-000000000003');

INSERT INTO FinancialAids (Id, AidName, Description, Amount, EligibilityCriteria, ApplicationDeadline, DepartmentID) VALUES
('B0000000-0000-0000-0000-000000000001', 'Hỗ trợ tài chính 2023', 'Dành cho SV khó khăn', 3000000.00, 'Thu nhập gia đình < 10M/năm', '2023-08-01', '20000000-0000-0000-0000-000000000003');

INSERT INTO StudentScholarships (StudentID, ScholarshipID, AwardDate) VALUES
('60000000-0000-0000-0000-000000000001', 'A0000000-0000-0000-0000-000000000001', '2023-09-01');

INSERT INTO StudentFinancialAids (StudentID, FinancialAidID, AwardDate) VALUES
('60000000-0000-0000-0000-000000000001', 'B0000000-0000-0000-0000-000000000001', '2023-09-01');

INSERT INTO Classroom (Id, RoomName, Capacity, Location, Status, Equipment) VALUES
('C0000000-0000-0000-0000-000000000001', 'Phòng 101', 30, 'Tòa A', 'Available', 'Máy chiếu, bảng trắng');

INSERT INTO Courses (Id, CourseCode, CourseName, Credits, Description, LearningOutcomes, DepartmentID) VALUES
('D0000000-0000-0000-0000-000000000001', 'CS101', 'Lập trình Cơ bản', 3, 'Giới thiệu về lập trình với C#', 'Hiểu biến, vòng lặp, hàm', '30000000-0000-0000-0000-000000000001'),
('D0000000-0000-0000-0000-000000000002', 'CS102', 'Cấu trúc Dữ liệu', 3, 'Các cấu trúc dữ liệu cơ bản', 'Hiểu danh sách, cây, đồ thị', '30000000-0000-0000-0000-000000000001');

INSERT INTO CoursePrerequisites (CourseID, PrerequisiteCourseID) VALUES
('D0000000-0000-0000-0000-000000000002', 'D0000000-0000-0000-0000-000000000001');

INSERT INTO CourseCorequisites (CourseID, CorequisiteCourseID) VALUES
('D0000000-0000-0000-0000-000000000001', 'D0000000-0000-0000-0000-000000000002');

INSERT INTO CourseSyllabi (Id, CourseID, SyllabusContent, Version, CreatedDate) VALUES
('E0000000-0000-0000-0000-000000000001', 'D0000000-0000-0000-0000-000000000001', 'Nội dung đề cương CS101: Tuần 1 - Biến và kiểu dữ liệu...', 'Fall 2023', '2023-08-01');

INSERT INTO Semesters (Id, SemesterName, StartDate, EndDate) VALUES
('F0000000-0000-0000-0000-000000000001', 'Kỳ Thu 2023', '2023-09-01', '2023-12-31');

INSERT INTO ClassSections (CourseID, SemesterID, ProfessorID, ClassroomID, Capacity, IsOnline) VALUES
('D0000000-0000-0000-0000-000000000001', 'F0000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000001', 30, 0);

INSERT INTO ClassSectionSchedules (ClassSectionID, DayOfWeek, StartTime, EndTime) VALUES
('G0000000-0000-0000-0000-000000000001', 'Mon', '09:00', '11:00'),
('G0000000-0000-0000-0000-000000000001', 'Wed', '09:00', '11:00');

INSERT INTO CourseRegistrations (StudentID, ClassSectionID, RegistrationDate, Status) VALUES
('60000000-0000-0000-0000-000000000001', 'G0000000-0000-0000-0000-000000000001', '2023-08-15', 'Registered');

INSERT INTO Grades (CourseRegistrationID, GradeValue, QualityPoints) VALUES
('I0000000-0000-0000-0000-000000000001', 'A', 4.0);

INSERT INTO MidtermEvaluations (CourseRegistrationID, Status, Recommendation) VALUES
('I0000000-0000-0000-0000-000000000001', 'Tốt', 'Tiếp tục phát huy');

INSERT INTO ServiceRequests (StudentID, ServiceType, RequestDate, Status, Details) VALUES
('60000000-0000-0000-0000-000000000001', 'Transcript', '2023-10-01', 'Approved', 'Yêu cầu bảng điểm chính thức');

INSERT INTO CourseEvaluations (ClassSectionID, StudentID, Rating, Comments, EvaluationDate) VALUES
('23000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000001', 4, 'Môn học rất hữu ích!', '2023-12-15');

INSERT INTO MajorCourses (MajorID, CourseID) VALUES
('50000000-0000-0000-0000-000000000001', 'D0000000-0000-0000-0000-000000000001'),
('50000000-0000-0000-0000-000000000001', 'D0000000-0000-0000-0000-000000000002');




