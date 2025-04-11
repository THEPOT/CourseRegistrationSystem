dotnet ef migrations add AddAcademicYearAndStatusToSemester --project CDQTSystem_Domain\CDQTSystem_Domain.csproj --startup-project CDQTSystem_API\CDQTSystem_API.csproj
dotnet ef database update --project CDQTSystem_Domain\CDQTSystem_Domain.csproj --startup-project CDQTSystem_API\CDQTSystem_API.csproj
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' 
               + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' 
               + name + '];' + CHAR(13)
FROM sys.foreign_keys;

EXEC sp_executesql @sql;

-- Sau đó xóa toàn bộ bảng
EXEC sp_MSforeachtable 'DROP TABLE ?';

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




