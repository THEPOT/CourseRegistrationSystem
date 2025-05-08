dotnet ef migrations add AddAcademicYearAndStatusToSemester --project CDQTSystem_Domain\CDQTSystem_Domain.csproj --startup-project CDQTSystem_API\CDQTSystem_API.csproj
dotnet ef database update --project CDQTSystem_Domain\CDQTSystem_Domain.csproj --startup-project CDQTSystem_API\CDQTSystem_API.csproj
dotnet ef dbcontext scaffold "Server=(local);uid=sa;pwd=12345;database=CDQTSystem;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --output-dir Entities



-- Create Roles table
CREATE TABLE [Roles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [RoleName] NVARCHAR(100) NOT NULL
);

-- Create Users table
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserCode VARCHAR(50) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [FullName] NVARCHAR(255) NOT NULL,
    [Gender] NVARCHAR(50) NULL,
    [Image] NVARCHAR(255) NULL,
    [DateOfBirth] DATE NULL,
    [PhoneNumber] NVARCHAR(50) NULL,
    [Address] NVARCHAR(255) NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
);

-- Create Departments table
CREATE TABLE [Departments] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [DepartmentName] NVARCHAR(255) NOT NULL
);

-- Create Majors table
CREATE TABLE [Majors] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [MajorName] NVARCHAR(255) NOT NULL,
    [RequiredCredits] INT NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Majors_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
);

-- Create Professors table
CREATE TABLE [Professors] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Professors_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Professors_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
);

-- Create AdministrativeStaffs table
CREATE TABLE [AdministrativeStaffs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_AdministrativeStaffs_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_AdministrativeStaffs_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
);

-- Create Students table
CREATE TABLE [Students] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [MajorId] UNIQUEIDENTIFIER NOT NULL,
    [EnrollmentDate] DATE NOT NULL,
    [AdmissionDate] DATE NULL,
    [AdmissionStatus] NVARCHAR(50) NULL,
    CONSTRAINT [FK_Students_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Students_Majors] FOREIGN KEY ([MajorId]) REFERENCES [Majors]([Id])
);

-- Create Courses table (inferred from relations)
CREATE TABLE [Courses] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CourseName] NVARCHAR(255) NOT NULL,
    [CourseCode] NVARCHAR(50) NOT NULL,
    [Credits] INT NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    LearningOutcomes VARCHAR(MAX),
    [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Courses_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id]),
);

CREATE TABLE MajorCourses (
    MajorID UNIQUEIDENTIFIER NOT NULL,
    CourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (MajorID, CourseID),
    FOREIGN KEY (MajorID) REFERENCES Majors(Id),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id)
);

CREATE TABLE CoursePrerequisites (
    CourseID UNIQUEIDENTIFIER NOT NULL,
    PrerequisiteCourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (CourseID, PrerequisiteCourseID),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (PrerequisiteCourseID) REFERENCES Courses(Id)
);

CREATE TABLE CourseCorequisites (
    CourseID UNIQUEIDENTIFIER NOT NULL,
    CorequisiteCourseID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (CourseID, CorequisiteCourseID),
    FOREIGN KEY (CourseID) REFERENCES Courses(Id),
    FOREIGN KEY (CorequisiteCourseID) REFERENCES Courses(Id)
);

-- Create CourseSyllabus table
CREATE TABLE [CourseSyllabi] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CourseId] UNIQUEIDENTIFIER NOT NULL,
    [SyllabusContent] NVARCHAR(MAX) NULL,
    [Version] NVARCHAR(50) NULL,
    [CreatedDate] DATETIME NOT NULL,
    [UpdatedDate] DATETIME NULL,
    CONSTRAINT [FK_CourseSyllabi_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id])
);

-- Create Semesters table
CREATE TABLE [Semesters] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [SemesterName] NVARCHAR(100) NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [AcademicYear] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL
);
CREATE TABLE Classroom (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoomName VARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    Location VARCHAR(100),
    Status VARCHAR(20),
    Equipment VARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Create ClassSections table (inferred from relations)
CREATE TABLE [ClassSections] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CourseId] UNIQUEIDENTIFIER NOT NULL,
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [ProfessorId] UNIQUEIDENTIFIER,
    [ClassroomId] UNIQUEIDENTIFIER,
    [MaxCapacity] INT NOT NULL,
    IsOnline BIT DEFAULT 0,
    CONSTRAINT [FK_ClassSections_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_ClassSections_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_ClassSections_Classrooms] FOREIGN KEY ([ClassroomId]) REFERENCES [Classroom]([Id]),
    CONSTRAINT [FK_ClassSections_Professors] FOREIGN KEY ([ProfessorId]) REFERENCES [Professors]([Id])
);

CREATE TABLE ClassSession (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ClassSectionId UNIQUEIDENTIFIER NOT NULL,
    Date DATE NOT NULL,
    DayOfWeek NVARCHAR(20) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- Normal, Cancelled, Rescheduled, MakeUp
    Note NVARCHAR(255) NULL,
    FOREIGN KEY (ClassSectionId) REFERENCES ClassSections(Id)
);

CREATE TABLE ClassSectionSchedules (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClassSectionID UNIQUEIDENTIFIER NOT NULL,
    DayOfWeek VARCHAR(10) NOT NULL CHECK (DayOfWeek IN ('Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun')),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (ClassSectionID) REFERENCES ClassSections(Id)
);


-- Create RegistrationPeriods table
CREATE TABLE [RegistrationPeriods] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [MaxCredits] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate] DATETIME NOT NULL,
    CONSTRAINT [FK_RegistrationPeriods_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_RegistrationPeriods_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id])
);

-- Create CourseRegistrations table (inferred from relations)
CREATE TABLE [CourseRegistrations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [ClassSectionId] UNIQUEIDENTIFIER NOT NULL,
    [RegistrationPeriodId] UNIQUEIDENTIFIER NOT NULL,
    [RegistrationDate] DATETIME NOT NULL,
    [TuitionStatus] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_CourseRegistrations_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_CourseRegistrations_ClassSections] FOREIGN KEY ([ClassSectionId]) REFERENCES [ClassSections]([Id]),
    CONSTRAINT [FK_CourseRegistrations_RegistrationPeriods] FOREIGN KEY ([RegistrationPeriodId]) REFERENCES [RegistrationPeriods]([Id])
);

CREATE TABLE [CourseEvaluationPeriod] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_CourseRegistrations_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
);


-- Create Grades table
CREATE TABLE [Grades] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CourseRegistrationId] UNIQUEIDENTIFIER NOT NULL,
    [GradeValue] NVARCHAR(10) NOT NULL,
    [QualityPoints] DECIMAL(3, 2) NOT NULL,
    CONSTRAINT [FK_Grades_CourseRegistrations] FOREIGN KEY ([CourseRegistrationId]) REFERENCES [CourseRegistrations]([Id])
);

CREATE TABLE [CourseEvaluations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [ProfessorId] UNIQUEIDENTIFIER NOT NULL,
    [CourseId] UNIQUEIDENTIFIER NOT NULL,
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [EvaluationDate] DATETIME NOT NULL,
    [OverallSatisfaction] NVARCHAR(50) NULL,
    [Comments] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_CourseEvaluations_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_CourseEvaluations_Professors] FOREIGN KEY ([ProfessorId]) REFERENCES [Professors]([Id]),
    CONSTRAINT [FK_CourseEvaluations_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_CourseEvaluations_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id])
);

CREATE TABLE [EvaluationQuestions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [QuestionText] NVARCHAR(500) NOT NULL,
    [QuestionTextLocalized] NVARCHAR(500) NULL,
    [QuestionType] NVARCHAR(50) NOT NULL,
    [Category] NVARCHAR(100) NULL, -- Nhóm câu hỏi (ví dụ: Punctuality, Teaching skills)
    [OrderIndex] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1
);

-- Thêm bảng để lưu các lựa chọn cho mỗi câu hỏi
CREATE TABLE [EvaluationOptions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [QuestionId] UNIQUEIDENTIFIER NOT NULL,
    [OptionText] NVARCHAR(200) NOT NULL,
    [OptionTextLocalized] NVARCHAR(200) NULL, -- Thêm để lưu lựa chọn song ngữ
    [OrderIndex] INT NOT NULL,
    CONSTRAINT [FK_EvaluationOptions_EvaluationQuestions] FOREIGN KEY ([QuestionId]) REFERENCES [EvaluationQuestions]([Id])
);

-- Sửa bảng EvaluationAnswers để lưu câu trả lời với lựa chọn cụ thể
CREATE TABLE [EvaluationAnswers] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EvaluationId] UNIQUEIDENTIFIER NOT NULL,
    [QuestionId] UNIQUEIDENTIFIER NOT NULL,
    [SelectedOptionId] UNIQUEIDENTIFIER NULL, -- Lựa chọn được chọn
    [TextAnswer] NVARCHAR(MAX) NULL, -- Dành cho câu trả lời dạng văn bản (nếu có)
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_EvaluationAnswers_CourseEvaluations] FOREIGN KEY ([EvaluationId]) REFERENCES [CourseEvaluations]([Id]),
    CONSTRAINT [FK_EvaluationAnswers_EvaluationQuestions] FOREIGN KEY ([QuestionId]) REFERENCES [EvaluationQuestions]([Id]),
    CONSTRAINT [FK_EvaluationAnswers_EvaluationOptions] FOREIGN KEY ([SelectedOptionId]) REFERENCES [EvaluationOptions]([Id])
);

-- Create ServiceRequests table
CREATE TABLE [ServiceRequests] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceType] NVARCHAR(100) NOT NULL,
    [RequestDate] DATETIME NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [Details] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_ServiceRequests_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id])
);

-- Create Scholarships table
CREATE TABLE [Scholarships] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ScholarshipName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [EligibilityCriteria] NVARCHAR(MAX) NULL,
    [ApplicationDeadline] DATE NULL,
    [DepartmentId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_Scholarships_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
);

-- Create StudentScholarships table
CREATE TABLE [StudentScholarships] (
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [ScholarshipId] UNIQUEIDENTIFIER NOT NULL,
    [AwardDate] DATE NOT NULL,
    PRIMARY KEY ([StudentId], [ScholarshipId]),
    CONSTRAINT [FK_StudentScholarships_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_StudentScholarships_Scholarships] FOREIGN KEY ([ScholarshipId]) REFERENCES [Scholarships]([Id])
);

-- Create FinancialAids table
CREATE TABLE [FinancialAids] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [AidName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [EligibilityCriteria] NVARCHAR(MAX) NULL,
    [ApplicationDeadline] DATE NULL,
    [DepartmentId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_FinancialAids_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
);

-- Create StudentFinancialAids table
CREATE TABLE [StudentFinancialAids] (
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [FinancialAidId] UNIQUEIDENTIFIER NOT NULL,
    [AwardDate] DATE NOT NULL,
    PRIMARY KEY ([StudentId], [FinancialAidId]),
    CONSTRAINT [FK_StudentFinancialAids_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_StudentFinancialAids_FinancialAids] FOREIGN KEY ([FinancialAidId]) REFERENCES [FinancialAids]([Id])
);

-- Create TuitionPolicies table
CREATE TABLE [TuitionPolicies] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [PolicyName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [EffectiveDate] DATE NOT NULL,
    [ExpirationDate] DATE NULL,
    [MajorId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_TuitionPolicies_Majors] FOREIGN KEY ([MajorId]) REFERENCES [Majors]([Id])
);

-- Create TuitionPeriods table
CREATE TABLE [TuitionPeriods] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate] DATETIME NOT NULL,
    CONSTRAINT [FK_TuitionPeriods_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_TuitionPeriods_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id])
);

-- Create StudentTuitions table
CREATE TABLE [StudentTuitions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [TuitionPolicyId] UNIQUEIDENTIFIER NULL,
    [TotalAmount] DECIMAL(18, 2) NOT NULL,
    [AmountPaid] DECIMAL(18, 2) NOT NULL,
    [DiscountAmount] DECIMAL(18, 2) NULL,
    [PaymentStatus] NVARCHAR(50) NOT NULL,
    [DueDate] DATETIME NOT NULL,
    [PaymentDate] DATETIME NULL,
    [Notes] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_StudentTuitions_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_StudentTuitions_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_StudentTuitions_TuitionPolicies] FOREIGN KEY ([TuitionPolicyId]) REFERENCES [TuitionPolicies]([Id])
);

-- Create MidtermEvaluationPeriods table
CREATE TABLE [MidtermEvaluationPeriods] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [MidtermEvaluationPeriodId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [IsActive] BIT NOT NULL,
    CONSTRAINT [FK_MidtermEvaluationPeriods_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_MidtermEvaluations_MidtermEvaluationPeriods] FOREIGN KEY ([MidtermEvaluationPeriodId]) REFERENCES [MidtermEvaluationPeriods]([Id])
);

-- Create MidtermEvaluations table
CREATE TABLE [MidtermEvaluations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [StudentId] UNIQUEIDENTIFIER NOT NULL,
    [ProfessorId] UNIQUEIDENTIFIER NOT NULL,
    [CourseId] UNIQUEIDENTIFIER NOT NULL,
    [ClassSectionId] UNIQUEIDENTIFIER NOT NULL,
    [SemesterId] UNIQUEIDENTIFIER NOT NULL,
    [Score] DECIMAL(5,2) NULL,
    [Comments] NVARCHAR(MAX) NULL,
    [Recommendation] NVARCHAR(MAX) NULL,
    [EvaluationDate] DATETIME NOT NULL,
    CONSTRAINT [FK_MidtermEvaluations_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]),
    CONSTRAINT [FK_MidtermEvaluations_Professors] FOREIGN KEY ([ProfessorId]) REFERENCES [Professors]([Id]),
    CONSTRAINT [FK_MidtermEvaluations_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_MidtermEvaluations_ClassSections] FOREIGN KEY ([ClassSectionId]) REFERENCES [ClassSections]([Id]),
    CONSTRAINT [FK_MidtermEvaluations_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id])
);



-- Insert sample Roles
INSERT INTO [Roles] ([Id], [RoleName]) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin'),
    ('22222222-2222-2222-2222-222222222222', 'Professor'),
    ('33333333-3333-3333-3333-333333333333', 'Student'),
    ('44444444-4444-4444-4444-444444444444', 'Administrative Staff');

-- Insert sample Departments
INSERT INTO [Departments] ([Id], [DepartmentName]) VALUES
    ('AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', 'Computer Science'),
    ('BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', 'Engineering'),
    ('CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', 'Mathematics'),
    ('DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD', 'Business'),
    ('EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE', 'Arts and Humanities');

-- Insert sample Users
INSERT INTO [Users] ([Id], [UserCode], [Email], [Password], [FullName], [Gender], [Image], [DateOfBirth], [PhoneNumber], [Address], [RoleId]) VALUES
    -- Admin
    ('AAAAAAAA-1111-1111-1111-111111111111', 'ADM001', 'admin@university.edu', 'hashed_password_1', 'Admin User', 'Male', 'admin.jpg', '1980-05-15', '555-123-4567', '123 Admin St, City', '11111111-1111-1111-1111-111111111111'),

    -- Professors
    ('BBBBBBBB-1111-1111-1111-111111111111', 'PROF001', 'smith@university.edu', 'hashed_password_2', 'John Smith', 'Male', 'smith.jpg', '1975-03-10', '555-234-5678', '456 Professor Ave, City', '22222222-2222-2222-2222-222222222222'),
    ('BBBBBBBB-2222-2222-2222-222222222222', 'PROF002', 'johnson@university.edu', 'hashed_password_3', 'Sarah Johnson', 'Female', 'johnson.jpg', '1980-07-22', '555-345-6789', '789 Faculty Dr, City', '22222222-2222-2222-2222-222222222222'),
    ('BBBBBBBB-3333-3333-3333-333333333333', 'PROF003', 'patel@university.edu', 'hashed_password_4', 'Raj Patel', 'Male', 'patel.jpg', '1978-11-15', '555-456-7890', '101 Academic Blvd, City', '22222222-2222-2222-2222-222222222222'),
    ('BBBBBBBB-4444-4444-4444-444444444444', 'PROF004', 'garcia@university.edu', 'hashed_password_5', 'Elena Garcia', 'Female', 'garcia.jpg', '1982-09-30', '555-567-8901', '202 Scholar St, City', '22222222-2222-2222-2222-222222222222'),

    -- Students
    ('CCCCCCCC-1111-1111-1111-111111111111', 'STU001', 'alice@university.edu', 'hashed_password_6', 'Alice Chen', 'Female', 'alice.jpg', '2000-01-15', '555-678-9012', '303 Student Dr, City', '33333333-3333-3333-3333-333333333333'),
    ('CCCCCCCC-2222-2222-2222-222222222222', 'STU002', 'david@university.edu', 'hashed_password_7', 'David Lee', 'Male', 'david.jpg', '1999-05-20', '555-789-0123', '404 College Ave, City', '33333333-3333-3333-3333-333333333333'),
    ('CCCCCCCC-3333-3333-3333-333333333333', 'STU003', 'maria@university.edu', 'hashed_password_8', 'Maria Rodriguez', 'Female', 'maria.jpg', '2001-03-10', '555-890-1234', '505 Campus Rd, City', '33333333-3333-3333-3333-333333333333'),
    ('CCCCCCCC-4444-4444-4444-444444444444', 'STU004', 'james@university.edu', 'hashed_password_9', 'James Wilson', 'Male', 'james.jpg', '2000-11-25', '555-901-2345', '606 University Blvd, City', '33333333-3333-3333-3333-333333333333'),
    ('CCCCCCCC-5555-5555-5555-555555555555', 'STU005', 'sophia@university.edu', 'hashed_password_10', 'Sophia Kim', 'Female', 'sophia.jpg', '2002-09-05', '555-012-3456', '707 Dormitory St, City', '33333333-3333-3333-3333-333333333333'),

    -- Administrative Staff
    ('DDDDDDDD-1111-1111-1111-111111111111', 'STAFF001', 'amanda@university.edu', 'hashed_password_11', 'Amanda Brown', 'Female', 'amanda.jpg', '1985-06-12', '555-123-7890', '808 Staff Rd, City', '44444444-4444-4444-4444-444444444444'),
    ('DDDDDDDD-2222-2222-2222-222222222222', 'STAFF002', 'michael@university.edu', 'hashed_password_12', 'Michael Thompson', 'Male', 'michael.jpg', '1990-12-03', '555-234-8901', '909 Admin Ave, City', '44444444-4444-4444-4444-444444444444');

-- Insert sample Majors
INSERT INTO [Majors] ([Id], [MajorName], [RequiredCredits], [DepartmentId]) VALUES
    ('AAAAAAAA-BBBB-1111-1111-111111111111', 'Computer Science', 120, 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('AAAAAAAA-BBBB-2222-2222-222222222222', 'Software Engineering', 124, 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('AAAAAAAA-BBBB-3333-3333-333333333333', 'Mechanical Engineering', 126, 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'),
    ('AAAAAAAA-BBBB-4444-4444-444444444444', 'Electrical Engineering', 128, 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'),
    ('AAAAAAAA-BBBB-5555-5555-555555555555', 'Applied Mathematics', 120, 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC'),
    ('AAAAAAAA-BBBB-6666-6666-666666666666', 'Business Administration', 120, 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD'),
    ('AAAAAAAA-BBBB-7777-7777-777777777777', 'Marketing', 120, 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD'),
    ('AAAAAAAA-BBBB-8888-8888-888888888888', 'English Literature', 120, 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE');

-- Insert sample Professors (linked to Users)
INSERT INTO [Professors] ([Id], [UserId], [DepartmentId]) VALUES
    ('AABBCCDD-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('AABBCCDD-2222-2222-2222-222222222222', 'BBBBBBBB-2222-2222-2222-222222222222', 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'),
    ('AABBCCDD-3333-3333-3333-333333333333', 'BBBBBBBB-3333-3333-3333-333333333333', 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC'),
    ('AABBCCDD-4444-4444-4444-444444444444', 'BBBBBBBB-4444-4444-4444-444444444444', 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD');

-- Insert sample AdministrativeStaffs
INSERT INTO [AdministrativeStaffs] ([Id], [UserId], [DepartmentId]) VALUES
    ('EEFFGGHH-1111-1111-1111-111111111111', 'DDDDDDDD-1111-1111-1111-111111111111', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('EEFFGGHH-2222-2222-2222-222222222222', 'DDDDDDDD-2222-2222-2222-222222222222', 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD');

-- Insert sample Students
INSERT INTO [Students] ([Id], [UserId], [MajorId], [EnrollmentDate], [AdmissionDate], [AdmissionStatus]) VALUES
    ('IIJJKKLL-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', 'AAAAAAAA-BBBB-1111-1111-111111111111', '2022-09-01', '2022-08-15', 'Active'),
    ('IIJJKKLL-2222-2222-2222-222222222222', 'CCCCCCCC-2222-2222-2222-222222222222', 'AAAAAAAA-BBBB-2222-2222-222222222222', '2022-09-01', '2022-08-10', 'Active'),
    ('IIJJKKLL-3333-3333-3333-333333333333', 'CCCCCCCC-3333-3333-3333-333333333333', 'AAAAAAAA-BBBB-6666-6666-666666666666', '2021-09-01', '2021-08-20', 'Active'),
    ('IIJJKKLL-4444-4444-4444-444444444444', 'CCCCCCCC-4444-4444-4444-444444444444', 'AAAAAAAA-BBBB-3333-3333-333333333333', '2023-09-01', '2023-08-18', 'Active'),
    ('IIJJKKLL-5555-5555-5555-555555555555', 'CCCCCCCC-5555-5555-5555-555555555555', 'AAAAAAAA-BBBB-8888-8888-888888888888', '2023-09-01', '2023-08-12', 'Active');

-- Insert sample Courses
INSERT INTO [Courses] ([Id], [CourseName], [CourseCode], [Credits], [Description], [LearningOutcomes], [DepartmentId]) VALUES
    ('MMNNOOPP-1111-1111-1111-111111111111', 'Introduction to Programming', 'CS101', 3, 'Fundamental concepts of programming', 'Understanding basic programming concepts, writing simple programs, debugging techniques', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('MMNNOOPP-2222-2222-2222-222222222222', 'Data Structures', 'CS201', 4, 'Advanced data structures and algorithms', 'Implementing complex data structures, analyzing algorithm efficiency, solving advanced problems', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('MMNNOOPP-3333-3333-3333-333333333333', 'Calculus I', 'MATH101', 4, 'Introduction to differential calculus', 'Understanding limits, derivatives, and applications of differentiation', 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC'),
    ('MMNNOOPP-4444-4444-4444-444444444444', 'Thermodynamics', 'MECH301', 3, 'Principles of thermodynamics and heat transfer', 'Understanding laws of thermodynamics, heat transfer mechanisms, energy conservation', 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'),
    ('MMNNOOPP-5555-5555-5555-555555555555', 'Marketing Principles', 'BUS201', 3, 'Introduction to marketing concepts and strategies', 'Understanding marketing mix, developing marketing plans, analyzing consumer behavior', 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD'),
    ('MMNNOOPP-6666-6666-6666-666666666666', 'World Literature', 'ENG202', 3, 'Survey of world literature', 'Analyzing literary works, understanding cultural contexts, developing critical thinking', 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE');

-- Insert sample MajorCourses
INSERT INTO MajorCourses (MajorID, CourseID) VALUES
    ('AAAAAAAA-BBBB-1111-1111-111111111111', 'MMNNOOPP-1111-1111-1111-111111111111'),
    ('AAAAAAAA-BBBB-1111-1111-111111111111', 'MMNNOOPP-2222-2222-2222-222222222222'),
    ('AAAAAAAA-BBBB-1111-1111-111111111111', 'MMNNOOPP-3333-3333-3333-333333333333'),
    ('AAAAAAAA-BBBB-2222-2222-222222222222', 'MMNNOOPP-1111-1111-1111-111111111111'),
    ('AAAAAAAA-BBBB-2222-2222-222222222222', 'MMNNOOPP-2222-2222-2222-222222222222'),
    ('AAAAAAAA-BBBB-3333-3333-333333333333', 'MMNNOOPP-3333-3333-3333-333333333333'),
    ('AAAAAAAA-BBBB-3333-3333-333333333333', 'MMNNOOPP-4444-4444-4444-444444444444'),
    ('AAAAAAAA-BBBB-6666-6666-666666666666', 'MMNNOOPP-5555-5555-5555-555555555555'),
    ('AAAAAAAA-BBBB-8888-8888-888888888888', 'MMNNOOPP-6666-6666-6666-666666666666');

-- Insert sample CoursePrerequisites
INSERT INTO CoursePrerequisites (CourseID, PrerequisiteCourseID) VALUES
    ('MMNNOOPP-2222-2222-2222-222222222222', 'MMNNOOPP-1111-1111-1111-111111111111');

-- Insert sample CourseSyllabi
INSERT INTO [CourseSyllabi] ([Id], [CourseId], [SyllabusContent], [Version], [CreatedDate], [UpdatedDate]) VALUES
    ('QQRRSSTT-1111-1111-1111-111111111111', 'MMNNOOPP-1111-1111-1111-111111111111', 'Weekly schedule, grading policy, required materials for Introduction to Programming', '1.0', '2023-08-01', NULL),
    ('QQRRSSTT-2222-2222-2222-222222222222', 'MMNNOOPP-2222-2222-2222-222222222222', 'Weekly schedule, grading policy, required materials for Data Structures', '1.0', '2023-08-02', NULL),
    ('QQRRSSTT-3333-3333-3333-333333333333', 'MMNNOOPP-3333-3333-3333-333333333333', 'Weekly schedule, grading policy, required materials for Calculus I', '1.0', '2023-08-03', NULL);

-- Insert sample Semesters
INSERT INTO [Semesters] ([Id], [SemesterName], [StartDate], [EndDate], [AcademicYear], [Status]) VALUES
    ('UUVVWWXX-1111-1111-1111-111111111111', 'Fall 2023', '2023-09-01', '2023-12-20', '2023-2024', 'Completed'),
    ('UUVVWWXX-2222-2222-2222-222222222222', 'Spring 2024', '2024-01-15', '2024-05-10', '2023-2024', 'Completed'),
    ('UUVVWWXX-3333-3333-3333-333333333333', 'Fall 2024', '2024-09-01', '2024-12-20', '2024-2025', 'Active'),
    ('UUVVWWXX-4444-4444-4444-444444444444', 'Spring 2025', '2025-01-15', '2025-05-10', '2024-2025', 'Planned');

-- Insert sample Classrooms
INSERT INTO Classroom ([Id], [RoomName], [Capacity], [Location], [Status], [Equipment]) VALUES
    ('YYZZ1122-1111-1111-1111-111111111111', 'A101', 40, 'Building A, Floor 1', 'Active', 'Projector, Whiteboard'),
    ('YYZZ1122-2222-2222-2222-222222222222', 'B205', 30, 'Building B, Floor 2', 'Active', 'Projector, Computers, Whiteboard'),
    ('YYZZ1122-3333-3333-3333-333333333333', 'C310', 100, 'Building C, Floor 3', 'Active', 'Projector, Sound System, Whiteboard'),
    ('YYZZ1122-4444-4444-4444-444444444444', 'D120', 25, 'Building D, Floor 1', 'Maintenance', 'Whiteboard');

-- Insert sample ClassSections
INSERT INTO [ClassSections] ([Id], [CourseId], [SemesterId], [ClassroomId], [ProfessorId], [Schedule], [Location], [MaxCapacity], [IsOnline]) VALUES
    ('3344AABB-1111-1111-1111-111111111111', 'MMNNOOPP-1111-1111-1111-111111111111', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-1111-1111-1111-111111111111', 'AABBCCDD-1111-1111-1111-111111111111', 'MWF 10:00-11:15', 'Building A', 40, 0),
    ('3344AABB-2222-2222-2222-222222222222', 'MMNNOOPP-2222-2222-2222-222222222222', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-2222-2222-2222-222222222222', 'AABBCCDD-1111-1111-1111-111111111111', 'TTh 13:00-14:15', 'Building B', 30, 0),
    ('3344AABB-3333-3333-3333-333333333333', 'MMNNOOPP-3333-3333-3333-333333333333', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-3333-3333-3333-333333333333', 'AABBCCDD-3333-3333-3333-333333333333', 'MWF 09:00-10:15', 'Building C', 80, 0),
    ('3344AABB-4444-4444-4444-444444444444', 'MMNNOOPP-4444-4444-4444-444444444444', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-1111-1111-1111-111111111111', 'AABBCCDD-2222-2222-2222-222222222222', 'TTh 15:00-16:15', 'Building A', 40, 0),
    ('3344AABB-5555-5555-5555-555555555555', 'MMNNOOPP-5555-5555-5555-555555555555', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-2222-2222-2222-222222222222', 'AABBCCDD-4444-4444-4444-444444444444', 'MW 14:00-15:15', 'Building B', 30, 0),
    ('3344AABB-6666-6666-6666-666666666666', 'MMNNOOPP-6666-6666-6666-666666666666', 'UUVVWWXX-1111-1111-1111-111111111111', 'YYZZ1122-3333-3333-3333-333333333333', 'AABBCCDD-4444-4444-4444-444444444444', 'F 10:00-12:45', 'Building C', 60, 0),
    -- Spring 2024 sections
    ('3344AABB-7777-7777-7777-777777777777', 'MMNNOOPP-1111-1111-1111-111111111111', 'UUVVWWXX-2222-2222-2222-222222222222', 'YYZZ1122-1111-1111-1111-111111111111', 'AABBCCDD-1111-1111-1111-111111111111', 'MWF 10:00-11:15', 'Building A', 40, 0),
    ('3344AABB-8888-8888-8888-888888888888', 'MMNNOOPP-2222-2222-2222-222222222222', 'UUVVWWXX-2222-2222-2222-222222222222', 'YYZZ1122-2222-2222-2222-222222222222', 'AABBCCDD-1111-1111-1111-111111111111', 'TTh 13:00-14:15', 'Building B', 30, 0);

-- Insert sample ClassSectionSchedules
INSERT INTO ClassSectionSchedules ([Id], [ClassSectionID], [DayOfWeek], [StartTime], [EndTime]) VALUES
    (NEWID(), '3344AABB-1111-1111-1111-111111111111', 'Mon', '10:00', '11:15'),
    (NEWID(), '3344AABB-1111-1111-1111-111111111111', 'Wed', '10:00', '11:15'),
    (NEWID(), '3344AABB-1111-1111-1111-111111111111', 'Fri', '10:00', '11:15'),
    (NEWID(), '3344AABB-2222-2222-2222-222222222222', 'Tue', '13:00', '14:15'),
    (NEWID(), '3344AABB-2222-2222-2222-222222222222', 'Thu', '13:00', '14:15'),
    (NEWID(), '3344AABB-3333-3333-3333-333333333333', 'Mon', '09:00', '10:15'),
    (NEWID(), '3344AABB-3333-3333-3333-333333333333', 'Wed', '09:00', '10:15'),
    (NEWID(), '3344AABB-3333-3333-3333-333333333333', 'Fri', '09:00', '10:15');

-- Insert sample RegistrationPeriods
INSERT INTO [RegistrationPeriods] ([Id], [SemesterId], [StartDate], [EndDate], [MaxCredits], [Status], [CreatedBy], [CreatedDate]) VALUES
    ('CCDDEEFF-1111-1111-1111-111111111111', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-08-01', '2023-08-20', 18, 'Closed', 'AAAAAAAA-1111-1111-1111-111111111111', '2023-07-01'),
    ('CCDDEEFF-2222-2222-2222-222222222222', 'UUVVWWXX-2222-2222-2222-222222222222', '2023-12-01', '2023-12-20', 18, 'Closed', 'AAAAAAAA-1111-1111-1111-111111111111', '2023-11-01'),
    ('CCDDEEFF-3333-3333-3333-333333333333', 'UUVVWWXX-3333-3333-3333-333333333333', '2024-08-01', '2024-08-20', 18, 'Active', 'AAAAAAAA-1111-1111-1111-111111111111', '2024-07-01');

-- Insert sample CourseRegistrations
INSERT INTO [CourseRegistrations] ([Id], [StudentId], [ClassSectionId], [RegistrationPeriodId], [RegistrationDate], [TuitionStatus], [Status]) VALUES
    ('GGHH1122-1111-1111-1111-111111111111', 'IIJJKKLL-1111-1111-1111-111111111111', '3344AABB-1111-1111-1111-111111111111', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-05', 'Paid', 'Completed'),
    ('GGHH1122-2222-2222-2222-222222222222', 'IIJJKKLL-1111-1111-1111-111111111111', '3344AABB-2222-2222-2222-222222222222', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-05', 'Paid', 'Completed'),
    ('GGHH1122-3333-3333-3333-333333333333', 'IIJJKKLL-1111-1111-1111-111111111111', '3344AABB-3333-3333-3333-333333333333', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-06', 'Paid', 'Completed'),
    ('GGHH1122-4444-4444-4444-444444444444', 'IIJJKKLL-2222-2222-2222-222222222222', '3344AABB-1111-1111-1111-111111111111', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-07', 'Paid', 'Completed'),
    ('GGHH1122-5555-5555-5555-555555555555', 'IIJJKKLL-2222-2222-2222-222222222222', '3344AABB-3333-3333-3333-333333333333', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-07', 'Paid', 'Completed'),
    ('GGHH1122-6666-6666-6666-666666666666', 'IIJJKKLL-3333-3333-3333-333333333333', '3344AABB-5555-5555-5555-555555555555', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-10', 'Paid', 'Completed'),
    ('GGHH1122-7777-7777-7777-777777777777', 'IIJJKKLL-3333-3333-3333-333333333333', '3344AABB-6666-6666-6666-666666666666', 'CCDDEEFF-1111-1111-1111-111111111111', '2023-08-10', 'Paid', 'Completed'),
    -- Spring 2024 registrations
    ('GGHH1122-8888-8888-8888-888888888888', 'IIJJKKLL-1111-1111-1111-111111111111', '3344AABB-7777-7777-7777-777777777777', 'CCDDEEFF-2222-2222-2222-222222222222', '2023-12-05', 'Paid', 'Completed'),
    ('GGHH1122-9999-9999-9999-999999999999', 'IIJJKKLL-1111-1111-1111-111111111111', '3344AABB-8888-8888-8888-888888888888', 'CCDDEEFF-2222-2222-2222-222222222222', '2023-12-05', 'Paid', 'Completed');

-- Insert sample CourseEvaluationPeriod
-- Insert sample CourseEvaluationPeriod (continued)
INSERT INTO [CourseEvaluationPeriod] ([Id], [SemesterId], [StartDate], [EndDate], [Status]) VALUES
    ('IIJJKKLL-1111-2222-3333-444444444444', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-12-01', '2023-12-15', 'Completed'),
    ('IIJJKKLL-2222-3333-4444-555555555555', 'UUVVWWXX-2222-2222-2222-222222222222', '2024-05-01', '2024-05-15', 'Completed'),
    ('IIJJKKLL-3333-4444-5555-666666666666', 'UUVVWWXX-3333-3333-3333-333333333333', '2024-12-01', '2024-12-15', 'Planned');

-- Insert sample Grades
INSERT INTO [Grades] ([Id], [CourseRegistrationId], [GradeValue], [QualityPoints]) VALUES
    ('MMNNOOPP-1111-2222-3333-444444444444', 'GGHH1122-1111-1111-1111-111111111111', 'A', 4.00),
    ('MMNNOOPP-2222-3333-4444-555555555555', 'GGHH1122-2222-2222-2222-222222222222', 'B+', 3.50),
    ('MMNNOOPP-3333-4444-5555-666666666666', 'GGHH1122-3333-3333-3333-333333333333', 'A-', 3.70),
    ('MMNNOOPP-4444-5555-6666-777777777777', 'GGHH1122-4444-4444-4444-444444444444', 'B', 3.00),
    ('MMNNOOPP-5555-6666-7777-888888888888', 'GGHH1122-5555-5555-5555-555555555555', 'A', 4.00),
    ('MMNNOOPP-6666-7777-8888-999999999999', 'GGHH1122-6666-6666-6666-666666666666', 'C+', 2.50),
    ('MMNNOOPP-7777-8888-9999-AAAAAAAAAAAA', 'GGHH1122-7777-7777-7777-777777777777', 'B-', 2.70),
    ('MMNNOOPP-8888-9999-AAAA-BBBBBBBBBBBB', 'GGHH1122-8888-8888-8888-888888888888', 'A', 4.00),
    ('MMNNOOPP-9999-AAAA-BBBB-CCCCCCCCCCCC', 'GGHH1122-9999-9999-9999-999999999999', 'B+', 3.50);

-- Insert sample EvaluationQuestions
INSERT INTO [EvaluationQuestions] ([Id], [QuestionText], [QuestionTextLocalized], [QuestionType], [Category], [OrderIndex], [IsActive]) VALUES
    ('QQRRSSTT-1111-2222-3333-444444444444', 'How would you rate the overall quality of this course?', 'Làm thế nào bạn đánh giá chất lượng tổng thể của khóa học này?', 'MultipleChoice', 'Overall Quality', 1, 1),
    ('QQRRSSTT-2222-3333-4444-555555555555', 'How effective was the professor in teaching this course?', 'Giảng viên có hiệu quả như thế nào trong việc giảng dạy khóa học này?', 'MultipleChoice', 'Teaching Effectiveness', 2, 1),
    ('QQRRSSTT-3333-4444-5555-666666666666', 'How relevant were the course materials to your learning?', 'Tài liệu khóa học có liên quan như thế nào đến việc học của bạn?', 'MultipleChoice', 'Course Materials', 3, 1),
    ('QQRRSSTT-4444-5555-6666-777777777777', 'How fair was the grading system?', 'Hệ thống chấm điểm có công bằng không?', 'MultipleChoice', 'Assessment', 4, 1),
    ('QQRRSSTT-5555-6666-7777-888888888888', 'Please provide any additional comments or suggestions.', 'Vui lòng cung cấp bất kỳ ý kiến ​​hoặc đề xuất bổ sung nào.', 'Text', 'Additional Comments', 5, 1);

-- Insert sample EvaluationOptions
INSERT INTO [EvaluationOptions] ([Id], [QuestionId], [OptionText], [OptionTextLocalized], [OrderIndex]) VALUES
    -- Options for question 1
    ('UUVVWWXX-1111-2222-3333-444444444444', 'QQRRSSTT-1111-2222-3333-444444444444', 'Excellent', 'Xuất sắc', 1),
    ('UUVVWWXX-2222-3333-4444-555555555555', 'QQRRSSTT-1111-2222-3333-444444444444', 'Good', 'Tốt', 2),
    ('UUVVWWXX-3333-4444-5555-666666666666', 'QQRRSSTT-1111-2222-3333-444444444444', 'Average', 'Trung bình', 3),
    ('UUVVWWXX-4444-5555-6666-777777777777', 'QQRRSSTT-1111-2222-3333-444444444444', 'Poor', 'Kém', 4),
    ('UUVVWWXX-5555-6666-7777-888888888888', 'QQRRSSTT-1111-2222-3333-444444444444', 'Very Poor', 'Rất kém', 5),

    -- Options for question 2
    ('UUVVWWXX-6666-7777-8888-999999999999', 'QQRRSSTT-2222-3333-4444-555555555555', 'Extremely Effective', 'Cực kỳ hiệu quả', 1),
    ('UUVVWWXX-7777-8888-9999-AAAAAAAAAAAA', 'QQRRSSTT-2222-3333-4444-555555555555', 'Very Effective', 'Rất hiệu quả', 2),
    ('UUVVWWXX-8888-9999-AAAA-BBBBBBBBBBBB', 'QQRRSSTT-2222-3333-4444-555555555555', 'Moderately Effective', 'Hiệu quả vừa phải', 3),
    ('UUVVWWXX-9999-AAAA-BBBB-CCCCCCCCCCCC', 'QQRRSSTT-2222-3333-4444-555555555555', 'Slightly Effective', 'Hơi hiệu quả', 4),
    ('UUVVWWXX-AAAA-BBBB-CCCC-DDDDDDDDDDDD', 'QQRRSSTT-2222-3333-4444-555555555555', 'Not Effective', 'Không hiệu quả', 5),

    -- Options for question 3
    ('UUVVWWXX-BBBB-CCCC-DDDD-EEEEEEEEEEEE', 'QQRRSSTT-3333-4444-5555-666666666666', 'Highly Relevant', 'Rất liên quan', 1),
    ('UUVVWWXX-CCCC-DDDD-EEEE-FFFFFFFFFFFF', 'QQRRSSTT-3333-4444-5555-666666666666', 'Relevant', 'Liên quan', 2),
    ('UUVVWWXX-DDDD-EEEE-FFFF-GGGGGGGGGGGG', 'QQRRSSTT-3333-4444-5555-666666666666', 'Somewhat Relevant', 'Hơi liên quan', 3),
    ('UUVVWWXX-EEEE-FFFF-GGGG-HHHHHHHHHHHH', 'QQRRSSTT-3333-4444-5555-666666666666', 'Not Very Relevant', 'Không quá liên quan', 4),
    ('UUVVWWXX-FFFF-GGGG-HHHH-IIIIIIIIIIII', 'QQRRSSTT-3333-4444-5555-666666666666', 'Not Relevant At All', 'Hoàn toàn không liên quan', 5),

    -- Options for question 4
    ('UUVVWWXX-GGGG-HHHH-IIII-JJJJJJJJJJJJ', 'QQRRSSTT-4444-5555-6666-777777777777', 'Very Fair', 'Rất công bằng', 1),
    ('UUVVWWXX-HHHH-IIII-JJJJ-KKKKKKKKKKKK', 'QQRRSSTT-4444-5555-6666-777777777777', 'Fair', 'Công bằng', 2),
    ('UUVVWWXX-IIII-JJJJ-KKKK-LLLLLLLLLLLL', 'QQRRSSTT-4444-5555-6666-777777777777', 'Neutral', 'Trung lập', 3),
    ('UUVVWWXX-JJJJ-KKKK-LLLL-MMMMMMMMMMMM', 'QQRRSSTT-4444-5555-6666-777777777777', 'Unfair', 'Không công bằng', 4),
    ('UUVVWWXX-KKKK-LLLL-MMMM-NNNNNNNNNNNN', 'QQRRSSTT-4444-5555-6666-777777777777', 'Very Unfair', 'Rất không công bằng', 5);

-- Insert sample CourseEvaluations
INSERT INTO [CourseEvaluations] ([Id], [StudentId], [ProfessorId], [CourseId], [SemesterId], [EvaluationDate], [OverallSatisfaction], [Comments]) VALUES
    ('AABBCCDD-1111-2222-3333-444444444444', 'IIJJKKLL-1111-1111-1111-111111111111', 'AABBCCDD-1111-1111-1111-111111111111', 'MMNNOOPP-1111-1111-1111-111111111111', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-12-10', 'Good', 'The course was well-structured and informative.'),
    ('AABBCCDD-2222-3333-4444-555555555555', 'IIJJKKLL-1111-1111-1111-111111111111', 'AABBCCDD-1111-1111-1111-111111111111', 'MMNNOOPP-2222-2222-2222-222222222222', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-12-10', 'Excellent', 'Challenging but very rewarding course.'),
    ('AABBCCDD-3333-4444-5555-666666666666', 'IIJJKKLL-2222-2222-2222-222222222222', 'AABBCCDD-1111-1111-1111-111111111111', 'MMNNOOPP-1111-1111-1111-111111111111', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-12-11', 'Average', 'The pace was a bit fast for beginners.'),
    ('AABBCCDD-4444-5555-6666-777777777777', 'IIJJKKLL-3333-3333-3333-333333333333', 'AABBCCDD-4444-4444-4444-444444444444', 'MMNNOOPP-5555-5555-5555-555555555555', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-12-12', 'Good', 'Practical examples were very helpful.');

-- Insert sample EvaluationAnswers
INSERT INTO [EvaluationAnswers] ([Id], [EvaluationId], [QuestionId], [SelectedOptionId], [TextAnswer], [CreatedDate]) VALUES
    -- Answers for evaluation 1
    ('EEFFGGHH-1111-2222-3333-444444444444', 'AABBCCDD-1111-2222-3333-444444444444', 'QQRRSSTT-1111-2222-3333-444444444444', 'UUVVWWXX-2222-3333-4444-555555555555', NULL, '2023-12-10'),
    ('EEFFGGHH-2222-3333-4444-555555555555', 'AABBCCDD-1111-2222-3333-444444444444', 'QQRRSSTT-2222-3333-4444-555555555555', 'UUVVWWXX-7777-8888-9999-AAAAAAAAAAAA', NULL, '2023-12-10'),
    ('EEFFGGHH-3333-4444-5555-666666666666', 'AABBCCDD-1111-2222-3333-444444444444', 'QQRRSSTT-3333-4444-5555-666666666666', 'UUVVWWXX-BBBB-CCCC-DDDD-EEEEEEEEEEEE', NULL, '2023-12-10'),
    ('EEFFGGHH-4444-5555-6666-777777777777', 'AABBCCDD-1111-2222-3333-444444444444', 'QQRRSSTT-4444-5555-6666-777777777777', 'UUVVWWXX-GGGG-HHHH-IIII-JJJJJJJJJJJJ', NULL, '2023-12-10'),
    ('EEFFGGHH-5555-6666-7777-888888888888', 'AABBCCDD-1111-2222-3333-444444444444', 'QQRRSSTT-5555-6666-7777-888888888888', NULL, 'The course was well-structured and informative.', '2023-12-10'),

    -- Answers for evaluation 2
    ('EEFFGGHH-6666-7777-8888-999999999999', 'AABBCCDD-2222-3333-4444-555555555555', 'QQRRSSTT-1111-2222-3333-444444444444', 'UUVVWWXX-1111-2222-3333-444444444444', NULL, '2023-12-10'),
    ('EEFFGGHH-7777-8888-9999-AAAAAAAAAAAA', 'AABBCCDD-2222-3333-4444-555555555555', 'QQRRSSTT-2222-3333-4444-555555555555', 'UUVVWWXX-6666-7777-8888-999999999999', NULL, '2023-12-10'),
    ('EEFFGGHH-8888-9999-AAAA-BBBBBBBBBBBB', 'AABBCCDD-2222-3333-4444-555555555555', 'QQRRSSTT-3333-4444-5555-666666666666', 'UUVVWWXX-BBBB-CCCC-DDDD-EEEEEEEEEEEE', NULL, '2023-12-10'),
    ('EEFFGGHH-9999-AAAA-BBBB-CCCCCCCCCCCC', 'AABBCCDD-2222-3333-4444-555555555555', 'QQRRSSTT-4444-5555-6666-777777777777', 'UUVVWWXX-GGGG-HHHH-IIII-JJJJJJJJJJJJ', NULL, '2023-12-10'),
    ('EEFFGGHH-AAAA-BBBB-CCCC-DDDDDDDDDDDD', 'AABBCCDD-2222-3333-4444-555555555555', 'QQRRSSTT-5555-6666-7777-888888888888', NULL, 'Challenging but very rewarding course.', '2023-12-10');

-- Insert sample ServiceRequests
INSERT INTO [ServiceRequests] ([Id], [StudentId], [ServiceType], [RequestDate], [Status], [Details]) VALUES
    ('IIJJKKLL-1111-2222-3333-444444444444', 'IIJJKKLL-1111-1111-1111-111111111111', 'Transcript Request', '2023-12-20', 'Completed', 'Need official transcript for job application'),
    ('IIJJKKLL-2222-3333-4444-555555555555', 'IIJJKKLL-2222-2222-2222-222222222222', 'Course Withdrawal', '2023-11-15', 'Approved', 'Need to withdraw from CS201 due to medical reasons'),
    ('IIJJKKLL-3333-4444-5555-666666666666', 'IIJJKKLL-3333-3333-3333-333333333333', 'Tuition Payment Plan', '2023-08-25', 'Approved', 'Requesting monthly payment plan for Fall 2023'),
    ('IIJJKKLL-4444-5555-6666-777777777777', 'IIJJKKLL-4444-4444-4444-444444444444', 'Enrollment Verification', '2024-02-10', 'Completed', 'Need verification for insurance purposes');

-- Insert sample Scholarships
INSERT INTO [Scholarships] ([Id], [ScholarshipName], [Description], [Amount], [EligibilityCriteria], [ApplicationDeadline], [DepartmentId]) VALUES
    ('MMNNOOPP-1111-2222-3333-444444444444', 'Academic Excellence Scholarship', 'Awarded to students with outstanding academic performance', 5000.00, 'GPA of 3.8 or higher, full-time enrollment', '2023-06-30', NULL),
    ('MMNNOOPP-2222-3333-4444-555555555555', 'Engineering Merit Scholarship', 'Awarded to exceptional engineering students', 3500.00, 'Engineering major, GPA of 3.5 or higher', '2023-06-30', 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'),
    ('MMNNOOPP-3333-4444-5555-666666666666', 'Computer Science Excellence Award', 'For outstanding CS students', 4000.00, 'CS major, demonstrated leadership', '2023-07-15', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA'),
    ('MMNNOOPP-4444-5555-6666-777777777777', 'Business Leadership Scholarship', 'For business students with leadership potential', 3000.00, 'Business major, active in student organizations', '2023-07-15', 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD');

-- Insert sample StudentScholarships
INSERT INTO [StudentScholarships] ([StudentId], [ScholarshipId], [AwardDate]) VALUES
    ('IIJJKKLL-1111-1111-1111-111111111111', 'MMNNOOPP-1111-2222-3333-444444444444', '2023-07-15'),
    ('IIJJKKLL-1111-1111-1111-111111111111', 'MMNNOOPP-3333-4444-5555-666666666666', '2023-08-01'),
    ('IIJJKKLL-2222-2222-2222-222222222222', 'MMNNOOPP-2222-3333-4444-555555555555', '2023-07-20'),
    ('IIJJKKLL-5555-5555-5555-555555555555', 'MMNNOOPP-1111-2222-3333-444444444444', '2023-07-25');

-- Insert sample FinancialAids
INSERT INTO [FinancialAids] ([Id], [AidName], [Description], [Amount], [EligibilityCriteria], [ApplicationDeadline], [DepartmentId]) VALUES
    ('QQRRSSTT-1111-2222-3333-444444444444', 'Need-Based Grant', 'Financial assistance based on financial need', 3000.00, 'Demonstrated financial need, full-time enrollment', '2023-06-15', NULL),
    ('QQRRSSTT-2222-3333-4444-555555555555', 'First-Generation Student Aid', 'Support for first-generation college students', 2500.00, 'First-generation college student, financial need', '2023-06-15', NULL),
    ('QQRRSSTT-3333-4444-5555-666666666666', 'International Student Aid', 'Financial assistance for international students', 2000.00, 'International student status, academic merit', '2023-06-30', NULL);

-- Insert sample StudentFinancialAids
INSERT INTO [StudentFinancialAids] ([StudentId], [FinancialAidId], [AwardDate]) VALUES
    ('IIJJKKLL-3333-3333-3333-333333333333', 'QQRRSSTT-1111-2222-3333-444444444444', '2023-07-01'),
    ('IIJJKKLL-4444-4444-4444-444444444444', 'QQRRSSTT-2222-3333-4444-555555555555', '2023-07-05'),
    ('IIJJKKLL-5555-5555-5555-555555555555', 'QQRRSSTT-3333-4444-5555-666666666666', '2023-07-15');

-- Insert sample TuitionPolicies
INSERT INTO [TuitionPolicies] ([Id], [PolicyName], [Description], [Amount], [EffectiveDate], [ExpirationDate], [MajorId]) VALUES
    ('UUVVWWXX-1111-2222-3333-444444444444', 'Standard Undergraduate Tuition', 'Regular per-credit tuition rate for undergraduate students', 350.00, '2023-08-01', '2024-07-31', NULL),
    ('UUVVWWXX-2222-3333-4444-555555555555', 'Engineering Program Tuition', 'Special tuition rate for engineering programs', 400.00, '2023-08-01', '2024-07-31', 'AAAAAAAA-BBBB-3333-3333-333333333333'),
    ('UUVVWWXX-3333-4444-5555-666666666666', 'Computer Science Program Tuition', 'Special tuition rate for CS programs', 380.00, '2023-08-01', '2024-07-31', 'AAAAAAAA-BBBB-1111-1111-111111111111'),
    ('UUVVWWXX-4444-5555-6666-777777777777', 'Arts and Humanities Tuition', 'Tuition rate for arts and humanities programs', 320.00, '2023-08-01', '2024-07-31', 'AAAAAAAA-BBBB-8888-8888-888888888888');

-- Insert sample TuitionPeriods
INSERT INTO [TuitionPeriods] ([Id], [SemesterId], [StartDate], [EndDate], [Status], [CreatedBy], [CreatedDate]) VALUES
    ('AABBCCDD-1111-2222-3333-444444444444', 'UUVVWWXX-1111-1111-1111-111111111111', '2023-08-01', '2023-09-15', 'Closed', 'AAAAAAAA-1111-1111-1111-111111111111', '2023-07-01'),
    ('AABBCCDD-2222-3333-4444-555555555555', 'UUVVWWXX-2222-2222-2222-222222222222', '2023-12-15', '2024-02-01', 'Closed', 'AAAAAAAA-1111-1111-1111-111111111111', '2023-11-01'),
    ('AABBCCDD-3333-4444-5555-666666666666', 'UUVVWWXX-3333-3333-3333-333333333333', '2024-08-01', '2024-09-15', 'Planned', 'AAAAAAAA-1111-1111-1111-111111111111', '2024-07-01');

-- Insert sample StudentTuitions
INSERT INTO [StudentTuitions] ([Id], [StudentId], [SemesterId], [TuitionPolicyId], [TotalAmount], [AmountPaid], [DiscountAmount], [PaymentStatus], [DueDate], [PaymentDate], [Notes]) VALUES
    ('EEFFGGHH-1111-2222-3333-444444444444', 'IIJJKKLL-1111-1111-1111-111111111111', 'UUVVWWXX-1111-1111-1111-111111111111', 'UUVVWWXX-3333-4444-5555-666666666666', 4560.00, 4560.00, 0.00, 'Paid', '2023-09-15', '2023-08-20', 'Fall 2023 tuition - 12 credits'),
    ('EEFFGGHH-2222-3333-4444-555555555555', 'IIJJKKLL-2222-2222-2222-222222222222', 'UUVVWWXX-1111-1111-1111-111111111111', 'UUVVWWXX-2222-3333-4444-555555555555', 4800.00, 4800.00, 0.00, 'Paid', '2023-09-15', '2023-08-25', 'Fall 2023 tuition - 12 credits'),
    ('EEFFGGHH-3333-4444-5555-666666666666', 'IIJJKKLL-3333-3333-3333-333333333333', 'UUVVWWXX-1111-1111-1111-111111111111', 'UUVVWWXX-1111-2222-3333-444444444444', 4200.00, 4200.00, 0.00, 'Paid', '2023-09-15', '2023-09-01', 'Fall 2023 tuition - 12 credits'),
    ('EEFFGGHH-4444-5555-6666-777777777777', 'IIJJKKLL-4444-4444-4444-444444444444', 'UUVVWWXX-1111-1111-1111-111111111111', 'UUVVWWXX-2222-3333-4444-555555555555', 4800.00, 4800.00, 0.00, 'Paid', '2023-09-15', '2023-08-15', 'Fall 2023 tuition - 12 credits'),
    ('EEFFGGHH-5555-6666-7777-888888888888', 'IIJJKKLL-5555-5555-5555-555555555555', 'UUVVWWXX-1111-1111-1111-111111111111', 'UUVVWWXX-4444-5555-6666-777777777777', 3840.00, 3840.00, 0.00, 'Paid', '2023-09-15', '2023-09-10', 'Fall 2023 tuition - 12 credits'),
    -- Spring 2024 tuition
    ('EEFFGGHH-6666-7777-8888-999999999999', 'IIJJKKLL-1111-1111-1111-111111111111', 'UUVVWWXX-2222-2222-2222-222222222222', 'UUVVWWXX-3333-4444-5555-666666666666', 4560.00, 4560.00, 0.00, 'Paid', '2024-02-01', '2024-01-15', 'Spring 2024 tuition - 12 credits');

-- Insert sample MidtermEvaluationPeriods
INSERT INTO [MidtermEvaluationPeriods] ([Id], [SemesterId], [MidtermEvaluationPeriodId], [StartDate], [EndDate], [IsActive]) VALUES
    ('IIJJ1122-1111-2222-3333-444444444444', 'UUVVWWXX-1111-1111-1111-111111111111', 'IIJJ1122-1111-2222-3333-444444444444', '2023-10-15', '2023-10-25', 0),
    ('IIJJ1122-2222-3333-4444-555555555555', 'UUVVWWXX-2222-2222-2222-222222222222', 'IIJJ1122-2222-3333-4444-555555555555', '2024-03-01', '2024-03-10', 0),
    ('IIJJ1122-3333-4444-5555-666666666666', 'UUVVWWXX-3333-3333-3333-333333333333', 'IIJJ1122-3333-4444-5555-666666666666', '2024-10-15', '2024-10-25', 0);