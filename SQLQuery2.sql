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
    [ClassroomId] UNIQUEIDENTIFIER NOT NULL,
    [ProfessorId] UNIQUEIDENTIFIER NOT NULL,
    [Schedule] NVARCHAR(255) NULL,
    [Location] NVARCHAR(255) NULL,
    [MaxCapacity] INT NOT NULL,
    IsOnline BIT DEFAULT 0,
    CONSTRAINT [FK_ClassSections_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_ClassSections_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]),
    CONSTRAINT [FK_ClassSections_Classrooms] FOREIGN KEY ([ClassroomId]) REFERENCES [Classroom]([Id]),
    CONSTRAINT [FK_ClassSections_Professors] FOREIGN KEY ([ProfessorId]) REFERENCES [Professors]([Id])
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





