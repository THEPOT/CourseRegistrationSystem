using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDQTSystem_Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearAndStatusToSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RoomName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Classroo__3214EC07BF78B0DD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    DepartmentName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__3214EC07BCD57E06", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RoleName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3214EC07D0B34CBE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SemesterName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Semester__3214EC077B57D336", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CourseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LearningOutcomes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Courses__3214EC077A949D04", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Courses__Departm__6EF57B66",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FinancialAids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AidName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EligibilityCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDeadline = table.Column<DateOnly>(type: "date", nullable: true),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Financia__3214EC07CD752F94", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Financial__Depar__5EBF139D",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    MajorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequiredCredits = table.Column<int>(type: "int", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Majors__3214EC0706A736F8", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Majors__Departme__44FF419A",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scholarships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ScholarshipName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EligibilityCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDeadline = table.Column<DateOnly>(type: "date", nullable: true),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Scholars__3214EC074C907D15", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Scholarsh__Depar__5AEE82B9",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Image = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC07D9E1546D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Users__RoleId__412EB0B6",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseCorequisites",
                columns: table => new
                {
                    CourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorequisiteCourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourseCo__BE20E9E6D9E6BBA6", x => new { x.CourseID, x.CorequisiteCourseID });
                    table.ForeignKey(
                        name: "FK__CourseCor__Coreq__76969D2E",
                        column: x => x.CorequisiteCourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CourseCor__Cours__75A278F5",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    CourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequisiteCourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CoursePr__E09340CD9DABCBD5", x => new { x.CourseID, x.PrerequisiteCourseID });
                    table.ForeignKey(
                        name: "FK__CoursePre__Cours__71D1E811",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CoursePre__Prere__72C60C4A",
                        column: x => x.PrerequisiteCourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseSyllabi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourseSy__3214EC07A907D901", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CourseSyl__Cours__7B5B524B",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MajorCourses",
                columns: table => new
                {
                    MajorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MajorCou__B92A68A96EB66004", x => new { x.MajorID, x.CourseID });
                    table.ForeignKey(
                        name: "FK__MajorCour__Cours__2BFE89A6",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__MajorCour__Major__2B0A656D",
                        column: x => x.MajorID,
                        principalTable: "Majors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TuitionPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PolicyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MajorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TuitionP__3214EC0745FBEDBD", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TuitionPo__Major__571DF1D5",
                        column: x => x.MajorID,
                        principalTable: "Majors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AdministrativeStaff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__3214EC07110B788D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Administr__Depar__534D60F1",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Administr__UserI__52593CB8",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Professo__3214EC07E1AB028E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Professor__Depar__4E88ABD4",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Professor__UserI__4D94879B",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrationPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SemesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Registra__3214EC071C5822B6", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Registrat__Creat__3E1D39E1",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Registrat__Semes__3D2915A8",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    MSSV = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AdmissionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AdmissionStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Students__3214EC072A641ED3", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Students__MajorI__49C3F6B7",
                        column: x => x.MajorID,
                        principalTable: "Majors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Students__UserID__48CFD27E",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TuitionPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SemesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TuitionP__3214EC07DE141B5E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TuitionPe__Creat__42E1EEFE",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TuitionPe__Semes__41EDCAC5",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassroomID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClassSec__3214EC07445812CF", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ClassSect__Class__0D7A0286",
                        column: x => x.ClassroomID,
                        principalTable: "Classroom",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ClassSect__Cours__0A9D95DB",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ClassSect__Profe__0C85DE4D",
                        column: x => x.ProfessorID,
                        principalTable: "Professors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ClassSect__Semes__0B91BA14",
                        column: x => x.SemesterID,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceR__3214EC079BC47696", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ServiceRe__Stude__236943A5",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentFinancialAids",
                columns: table => new
                {
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinancialAidID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwardDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StudentF__2337769D9F585E8C", x => new { x.StudentID, x.FinancialAidID });
                    table.ForeignKey(
                        name: "FK__StudentFi__Finan__66603565",
                        column: x => x.FinancialAidID,
                        principalTable: "FinancialAids",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__StudentFi__Stude__656C112C",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentScholarships",
                columns: table => new
                {
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScholarshipID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwardDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StudentS__EA96C6580B50F8F2", x => new { x.StudentID, x.ScholarshipID });
                    table.ForeignKey(
                        name: "FK__StudentSc__Schol__628FA481",
                        column: x => x.ScholarshipID,
                        principalTable: "Scholarships",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__StudentSc__Stude__619B8048",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentTuition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuitionPolicyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentDueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StudentT__3214EC071197F40B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__StudentTu__Semes__04E4BC85",
                        column: x => x.SemesterID,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__StudentTu__Stude__03F0984C",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__StudentTu__Tuiti__05D8E0BE",
                        column: x => x.TuitionPolicyID,
                        principalTable: "TuitionPolicies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tuitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tuitions__3214EC070E564B1A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Tuitions__Semest__489AC854",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Tuitions__Studen__47A6A41B",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassSectionSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ClassSectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClassSec__3214EC0712BFD349", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ClassSect__Class__123EB7A3",
                        column: x => x.ClassSectionID,
                        principalTable: "ClassSections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ClassSectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourseEv__3214EC076F94292C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CourseEva__Class__2739D489",
                        column: x => x.ClassSectionID,
                        principalTable: "ClassSections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CourseEva__Stude__282DF8C2",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassSectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TuitionStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "Pending"),
                    RegistrationPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourseRe__3214EC07FD84D7CD", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CourseReg__Class__17036CC0",
                        column: x => x.ClassSectionID,
                        principalTable: "ClassSections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CourseReg__Regis__4A8310C6",
                        column: x => x.RegistrationPeriodId,
                        principalTable: "RegistrationPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CourseReg__Stude__160F4887",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CourseRegistrationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeValue = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    QualityPoints = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grades__3214EC07EB54789C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Grades__CourseRe__1BC821DD",
                        column: x => x.CourseRegistrationID,
                        principalTable: "CourseRegistrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MidtermEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CourseRegistrationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MidtermE__3214EC0733774C4A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__MidtermEv__Cours__1F98B2C1",
                        column: x => x.CourseRegistrationID,
                        principalTable: "CourseRegistrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativeStaff_DepartmentID",
                table: "AdministrativeStaff",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativeStaff_UserID",
                table: "AdministrativeStaff",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSections_ClassroomID",
                table: "ClassSections",
                column: "ClassroomID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSections_CourseID",
                table: "ClassSections",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSections_ProfessorID",
                table: "ClassSections",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSections_SemesterID",
                table: "ClassSections",
                column: "SemesterID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSectionSchedules_ClassSectionID",
                table: "ClassSectionSchedules",
                column: "ClassSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCorequisites_CorequisiteCourseID",
                table: "CourseCorequisites",
                column: "CorequisiteCourseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluations_ClassSectionID",
                table: "CourseEvaluations",
                column: "ClassSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluations_StudentID",
                table: "CourseEvaluations",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_PrerequisiteCourseID",
                table: "CoursePrerequisites",
                column: "PrerequisiteCourseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_ClassSectionID",
                table: "CourseRegistrations",
                column: "ClassSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_RegistrationPeriodId",
                table: "CourseRegistrations",
                column: "RegistrationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_StudentID",
                table: "CourseRegistrations",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentID",
                table: "Courses",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "UQ__Courses__FC00E000C9449B35",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSyllabi_CourseID",
                table: "CourseSyllabi",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "UQ__Departme__D949CC34AFE0707A",
                table: "Departments",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAids_DepartmentID",
                table: "FinancialAids",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CourseRegistrationID",
                table: "Grades",
                column: "CourseRegistrationID");

            migrationBuilder.CreateIndex(
                name: "IX_MajorCourses_CourseID",
                table: "MajorCourses",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Majors_DepartmentID",
                table: "Majors",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_MidtermEvaluations_CourseRegistrationID",
                table: "MidtermEvaluations",
                column: "CourseRegistrationID");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_DepartmentID",
                table: "Professors",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_UserID",
                table: "Professors",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_CreatedBy",
                table: "RegistrationPeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_SemesterId",
                table: "RegistrationPeriods",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__8A2B6160ED56A0DA",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scholarships_DepartmentID",
                table: "Scholarships",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_StudentID",
                table: "ServiceRequests",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFinancialAids_FinancialAidID",
                table: "StudentFinancialAids",
                column: "FinancialAidID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_MajorID",
                table: "Students",
                column: "MajorID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserID",
                table: "Students",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScholarships_ScholarshipID",
                table: "StudentScholarships",
                column: "ScholarshipID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTuition_SemesterID",
                table: "StudentTuition",
                column: "SemesterID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTuition_StudentID",
                table: "StudentTuition",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTuition_TuitionPolicyID",
                table: "StudentTuition",
                column: "TuitionPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPeriods_CreatedBy",
                table: "TuitionPeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPeriods_SemesterId",
                table: "TuitionPeriods",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPolicies_MajorID",
                table: "TuitionPolicies",
                column: "MajorID");

            migrationBuilder.CreateIndex(
                name: "IX_Tuitions_SemesterId",
                table: "Tuitions",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tuitions_StudentId",
                table: "Tuitions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D10534583BA837",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrativeStaff");

            migrationBuilder.DropTable(
                name: "ClassSectionSchedules");

            migrationBuilder.DropTable(
                name: "CourseCorequisites");

            migrationBuilder.DropTable(
                name: "CourseEvaluations");

            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "CourseSyllabi");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "MajorCourses");

            migrationBuilder.DropTable(
                name: "MidtermEvaluations");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "StudentFinancialAids");

            migrationBuilder.DropTable(
                name: "StudentScholarships");

            migrationBuilder.DropTable(
                name: "StudentTuition");

            migrationBuilder.DropTable(
                name: "TuitionPeriods");

            migrationBuilder.DropTable(
                name: "Tuitions");

            migrationBuilder.DropTable(
                name: "CourseRegistrations");

            migrationBuilder.DropTable(
                name: "FinancialAids");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropTable(
                name: "TuitionPolicies");

            migrationBuilder.DropTable(
                name: "ClassSections");

            migrationBuilder.DropTable(
                name: "RegistrationPeriods");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Classroom");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Professors");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
