-- -- Tạo cơ sở dữ liệu
-- CREATE DATABASE UniversityDb;
-- GO
-- -- dotnet ef dbcontext scaffold "Server=(local);uid=sa;pwd=12345;database=UniversityDb;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models
-- -- Tạo cơ sở dữ liệu
-- USE UniversityDb;
-- GO

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

-- Bảng khoa/bộ môn
CREATE TABLE Faculties (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FacultyName VARCHAR(100) UNIQUE NOT NULL
);

-- Bảng người dùng
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Image VARCHAR(MAX),
    RoleId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Bảng ngành học
CREATE TABLE Programs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramName VARCHAR(100) NOT NULL,
    RequiredCredits INT NOT NULL,
    FacultyID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (FacultyID) REFERENCES Faculties(Id)
);

-- Bảng sinh viên
CREATE TABLE Students (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MSSV VARCHAR(100) NOT NULL,
    UserID UNIQUEIDENTIFIER NOT NULL,
    ProgramID UNIQUEIDENTIFIER NOT NULL,
    EnrollmentDate DATE NOT NULL,
    AdmissionDate DATE,
    AdmissionStatus VARCHAR(20),
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (ProgramID) REFERENCES Programs(Id)
);

-- Bảng giảng viên
CREATE TABLE Lecturers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    FacultyID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (FacultyID) REFERENCES Faculties(Id)
);

-- Bảng nhân viên
CREATE TABLE Staff (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    DepartmentID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(Id),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng chính sách học phí
CREATE TABLE TuitionPolicies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PolicyName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EffectiveDate DATE NOT NULL,
    ExpirationDate DATE,
    ProgramID UNIQUEIDENTIFIER,
    FOREIGN KEY (ProgramID) REFERENCES Programs(Id)
);



-- Bảng học bổng
CREATE TABLE Scholarships (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScholarshipName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EligibilityCriteria VARCHAR(MAX),
    ApplicationDeadline DATE,
    DepartmentID UNIQUEIDENTIFIER,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(Id)
);

-- Bảng hỗ trợ tài chính
CREATE TABLE FinancialAids (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AidName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    Amount DECIMAL(18,2) NOT NULL,
    EligibilityCriteria VARCHAR(MAX),
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

-- Bảng môn học
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseCode VARCHAR(20) UNIQUE NOT NULL,
    CourseName VARCHAR(100) NOT NULL,
    Credits INT NOT NULL,
    Description VARCHAR(MAX),
    LearningOutcomes VARCHAR(MAX),
    FacultyID UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (FacultyID) REFERENCES Faculties(Id)
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
    SyllabusContent VARCHAR(MAX),
    Version VARCHAR(50),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    FOREIGN KEY (CourseID) REFERENCES Courses(Id)
);

-- Bảng kỳ học
CREATE TABLE Terms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TermName VARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL
);

-- Bảng học phí sinh viên
CREATE TABLE StudentTuition (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    TermID UNIQUEIDENTIFIER NOT NULL,
    TuitionPolicyID UNIQUEIDENTIFIER NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    AmountPaid DECIMAL(18,2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18,2) DEFAULT 0,
    PaymentStatus VARCHAR(20) NOT NULL CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid', 'Exempted')),
    PaymentDueDate DATE NOT NULL,
    PaymentDate DATETIME,
    Notes VARCHAR(MAX),
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (TermID) REFERENCES Terms(Id),
    FOREIGN KEY (TuitionPolicyID) REFERENCES TuitionPolicies(Id)
);

-- Bảng mở lớp học
CREATE TABLE CourseOfferings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseID UNIQUEIDENTIFIER NOT NULL,
    TermID UNIQUEIDENTIFIER NOT NULL,
    LecturerID UNIQUEIDENTIFIER NOT NULL,
    Classroom VARCHAR(50),
    Schedule VARCHAR(255),
    Capacity INT NOT NULL,
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (TermID) REFERENCES Terms(Id),
    FOREIGN KEY (LecturerID) REFERENCES Lecturers(Id)
);

-- Bảng đăng ký môn học
CREATE TABLE Registrations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    CourseOfferingID UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate DATETIME NOT NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Registered', 'Waitlisted', 'Dropped')),
    FOREIGN KEY (StudentID) REFERENCES Students(Id),
    FOREIGN KEY (CourseOfferingID) REFERENCES CourseOfferings(Id)
);

-- Bảng điểm số
CREATE TABLE Grades (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RegistrationID UNIQUEIDENTIFIER NOT NULL,
    GradeValue VARCHAR(5) NOT NULL CHECK (GradeValue IN ('A', 'B', 'C', 'D', 'F')),
    QualityPoints DECIMAL(5,2) NOT NULL,
    FOREIGN KEY (RegistrationID) REFERENCES Registrations(Id)
);

-- Bảng đánh giá giữa kỳ
CREATE TABLE MidtermEvaluations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RegistrationID UNIQUEIDENTIFIER NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Recommendation VARCHAR(MAX),
    FOREIGN KEY (RegistrationID) REFERENCES Registrations(Id)
);

-- Bảng yêu cầu dịch vụ
CREATE TABLE ServiceRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentID UNIQUEIDENTIFIER NOT NULL,
    ServiceType VARCHAR(50) NOT NULL CHECK (ServiceType IN ('Certificate', 'Transcript', 'LeaveOfAbsence', 'CreditOverload', 'ProgramChange', 'AddDrop', 'Withdraw', 'Graduation', 'AcademicAdvising', 'ClassroomBorrow', 'TemporaryWithdraw', 'PermanentWithdraw', 'Other')),
    RequestDate DATETIME NOT NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'Approved', 'Denied')),
    Details VARCHAR(MAX),
    FOREIGN KEY (StudentID) REFERENCES Students(Id)
);

-- Bảng đánh giá môn học cuối kỳ
CREATE TABLE CourseEvaluations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseOfferingID UNIQUEIDENTIFIER NOT NULL,
    StudentID UNIQUEIDENTIFIER NOT NULL,
    Rating INT NOT NULL,
    Comments VARCHAR(MAX),
    EvaluationDate DATETIME NOT NULL,
    FOREIGN KEY (CourseOfferingID) REFERENCES CourseOfferings(Id),
    FOREIGN KEY (StudentID) REFERENCES Students(Id)
);

-- Bảng liên kết giữa ngành học và môn học
CREATE TABLE ProgramCourses (
    ProgramID UNIQUEIDENTIFIER NOT NULL,
    CourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (ProgramID, CourseID),
    FOREIGN KEY (ProgramID) REFERENCES Programs(Id),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id)
);

INSERT INTO Roles (Id, RoleName) VALUES
('10000000-0000-0000-0000-000000000001', 'Admin'),
('10000000-0000-0000-0000-000000000002', 'Student'),
('10000000-0000-0000-0000-000000000003', 'Lecturer'),
('10000000-0000-0000-0000-000000000004', 'Staff');

INSERT INTO Departments (Id, DepartmentName) VALUES
('20000000-0000-0000-0000-000000000001', 'Phòng Đào tạo (PĐT)'),
('20000000-0000-0000-0000-000000000002', 'Phòng Kế toán'),
('20000000-0000-0000-0000-000000000003', 'Phòng Công tác Sinh viên (CTSV)');

INSERT INTO Faculties (Id, FacultyName) VALUES
('30000000-0000-0000-0000-000000000001', 'Khoa Công nghệ Thông tin'),
('30000000-0000-0000-0000-000000000002', 'Khoa Quản trị Kinh doanh');

INSERT INTO Users (Id, Email, Password, FullName, Image, RoleId) VALUES
('40000000-0000-0000-0000-000000000001', 'admin@university.com', 'hashed_pass1', 'Admin User', NULL, '10000000-0000-0000-0000-000000000001'),
('40000000-0000-0000-0000-000000000002', 'student1@university.com', 'hashed_pass2', 'Nguyen Van A', NULL, '10000000-0000-0000-0000-000000000002'),
('40000000-0000-0000-0000-000000000003', 'lecturer1@university.com', 'hashed_pass3', 'Tran Thi B', NULL, '10000000-0000-0000-0000-000000000003'),
('40000000-0000-0000-0000-000000000004', 'staff_pdt@university.com', 'hashed_pass4', 'Le Van C', NULL, '10000000-0000-0000-0000-000000000004'),
('40000000-0000-0000-0000-000000000005', 'staff_ketoan@university.com', 'hashed_pass5', 'Pham Thi D', NULL, '10000000-0000-0000-0000-000000000004');

INSERT INTO Programs (Id, ProgramName, RequiredCredits, FacultyID) VALUES
('50000000-0000-0000-0000-000000000001', 'Cử nhân Công nghệ Thông tin', 120, '30000000-0000-0000-0000-000000000001'),
('50000000-0000-0000-0000-000000000002', 'Cử nhân Quản trị Kinh doanh', 130, '30000000-0000-0000-0000-000000000002');


INSERT INTO Students (Id, MSSV, UserID, ProgramID, EnrollmentDate, AdmissionDate, AdmissionStatus) VALUES
('60000000-0000-0000-0000-000000000001', 'SV001', '40000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', '2023-09-01', '2023-08-01', 'Accepted');

INSERT INTO Lecturers (Id, UserID, FacultyID) VALUES
('70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001');

INSERT INTO Staff (Id, UserID, DepartmentID) VALUES
('80000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000001'), -- PĐT
('80000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002'); -- Kế toán

INSERT INTO TuitionPolicies (Id, PolicyName, Description, Amount, EffectiveDate, ExpirationDate, ProgramID) VALUES
('90000000-0000-0000-0000-000000000001', 'Học phí CNTT 2023', 'Học phí theo tín chỉ', 500000.00, '2023-01-01', NULL, '50000000-0000-0000-0000-000000000001');

INSERT INTO Scholarships (Id, ScholarshipName, Description, Amount, EligibilityCriteria, ApplicationDeadline, DepartmentID) VALUES
('A0000000-0000-0000-0000-000000000001', 'Học bổng Xuất sắc 2023', 'Dành cho SV top 10%', 5000000.00, 'GPA > 3.5', '2023-07-15', '20000000-0000-0000-0000-000000000003');

INSERT INTO FinancialAids (Id, AidName, Description, Amount, EligibilityCriteria, ApplicationDeadline, DepartmentID) VALUES
('B0000000-0000-0000-0000-000000000001', 'Hỗ trợ tài chính 2023', 'Dành cho SV khó khăn', 3000000.00, 'Thu nhập gia đình < 10M/năm', '2023-08-01', '20000000-0000-0000-0000-000000000003');

INSERT INTO StudentScholarships (StudentID, ScholarshipID, AwardDate) VALUES
('60000000-0000-0000-0000-000000000001', 'A0000000-0000-0000-0000-000000000001', '2023-09-01');

INSERT INTO StudentFinancialAids (StudentID, FinancialAidID, AwardDate) VALUES
('60000000-0000-0000-0000-000000000001', 'B0000000-0000-0000-0000-000000000001', '2023-09-01');

INSERT INTO Courses (Id, CourseCode, CourseName, Credits, Description, LearningOutcomes, FacultyID) VALUES
('C0000000-0000-0000-0000-000000000001', 'CS101', 'Lập trình Cơ bản', 3, 'Giới thiệu về lập trình với C#', 'Hiểu biến, vòng lặp, hàm', '30000000-0000-0000-0000-000000000001'),
('C0000000-0000-0000-0000-000000000002', 'CS102', 'Cấu trúc Dữ liệu', 3, 'Các cấu trúc dữ liệu cơ bản', 'Hiểu danh sách, cây, đồ thị', '30000000-0000-0000-0000-000000000001');

INSERT INTO CoursePrerequisites (CourseID, PrerequisiteCourseID) VALUES
('C0000000-0000-0000-0000-000000000002', 'C0000000-0000-0000-0000-000000000001'); -- CS102 yêu cầu CS101

INSERT INTO CourseCorequisites (CourseID, CorequisiteCourseID) VALUES
('C0000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000002'); -- CS101 và CS102 học cùng

INSERT INTO CourseSyllabi (Id, CourseID, SyllabusContent, Version, CreatedDate) VALUES
('D0000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000001', 'Nội dung đề cương CS101: Tuần 1 - Biến và kiểu dữ liệu...', 'Fall 2023', '2023-08-01');

INSERT INTO Terms (Id, TermName, StartDate, EndDate) VALUES
('E0000000-0000-0000-0000-000000000001', 'Kỳ Thu 2023', '2023-09-01', '2023-12-31');

INSERT INTO CourseOfferings (Id, CourseID, TermID, LecturerID, Classroom, Schedule, Capacity) VALUES
('F0000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000001', 'E0000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Phòng 101', 'T2-T4 9:00-11:00', 30);

INSERT INTO Registrations (Id, StudentID, CourseOfferingID, RegistrationDate, Status) VALUES
('G0000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000001', 'F0000000-0000-0000-0000-000000000001', '2023-08-15', 'Registered');

INSERT INTO Grades (Id, RegistrationID, GradeValue, QualityPoints) VALUES
('H0000000-0000-0000-0000-000000000001', 'G0000000-0000-0000-0000-000000000001', 'A', 4.0);

INSERT INTO MidtermEvaluations (Id, RegistrationID, Status, Recommendation) VALUES
('I0000000-0000-0000-0000-000000000001', 'G0000000-0000-0000-0000-000000000001', 'Tốt', 'Tiếp tục phát huy');

INSERT INTO ServiceRequests (Id, StudentID, ServiceType, RequestDate, Status, Details) VALUES
('J0000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000001', 'Transcript', '2023-10-01', 'Approved', 'Yêu cầu bảng điểm chính thức');

INSERT INTO CourseEvaluations (Id, CourseOfferingID, StudentID, Rating, Comments, EvaluationDate) VALUES
('K0000000-0000-0000-0000-000000000001', 'F0000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000001', 4, 'Môn học rất hữu ích!', '2023-12-15');

INSERT INTO ProgramCourses (ProgramID, CourseID) VALUES
('50000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000001'), -- CNTT yêu cầu CS101
('50000000-0000-0000-0000-000000000001', 'C0000000-0000-0000-0000-000000000002'); -- CNTT yêu cầu CS102















