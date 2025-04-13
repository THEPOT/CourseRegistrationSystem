using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_Domain.Entities;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext()
    {
    }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdministrativeStaff> AdministrativeStaffs { get; set; }

    public virtual DbSet<ClassSection> ClassSections { get; set; }

    public virtual DbSet<ClassSectionSchedule> ClassSectionSchedules { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEvaluation> CourseEvaluations { get; set; }

    public virtual DbSet<CourseRegistration> CourseRegistrations { get; set; }

    public virtual DbSet<CourseSyllabus> CourseSyllabi { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<FinancialAid> FinancialAids { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<MidtermEvaluation> MidtermEvaluations { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<RegistrationPeriod> RegistrationPeriods { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Scholarship> Scholarships { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentFinancialAid> StudentFinancialAids { get; set; }

    public virtual DbSet<StudentScholarship> StudentScholarships { get; set; }

    public virtual DbSet<StudentTuition> StudentTuitions { get; set; }

    public virtual DbSet<Tuition> Tuitions { get; set; }

    public virtual DbSet<TuitionPeriod> TuitionPeriods { get; set; }

    public virtual DbSet<TuitionPolicy> TuitionPolicies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=UniversityDb;TrustServerCertificate=True",
            options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministrativeStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC07110B788D");

            entity.ToTable("AdministrativeStaff");

            entity.HasIndex(e => e.DepartmentId, "IX_AdministrativeStaff_DepartmentID");

            entity.HasIndex(e => e.UserId, "IX_AdministrativeStaff_UserID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Department).WithMany(p => p.AdministrativeStaffs)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__Depar__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.AdministrativeStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__UserI__52593CB8");
        });

        modelBuilder.Entity<ClassSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC07445812CF");

            entity.HasIndex(e => e.ClassroomId, "IX_ClassSections_ClassroomID");

            entity.HasIndex(e => e.CourseId, "IX_ClassSections_CourseID");

            entity.HasIndex(e => e.ProfessorId, "IX_ClassSections_ProfessorID");

            entity.HasIndex(e => e.SemesterId, "IX_ClassSections_SemesterID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassroomId).HasColumnName("ClassroomID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);
            entity.Property(e => e.ProfessorId).HasColumnName("ProfessorID");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ClassroomId)
                .HasConstraintName("FK__ClassSect__Class__0D7A0286");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Cours__0A9D95DB");

            entity.HasOne(d => d.Professor).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("FK__ClassSect__Profe__0C85DE4D");

            entity.HasOne(d => d.Semester).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Semes__0B91BA14");
        });

        modelBuilder.Entity<ClassSectionSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC0712BFD349");

            entity.HasIndex(e => e.ClassSectionId, "IX_ClassSectionSchedules_ClassSectionID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ClassSectionSchedules)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Class__123EB7A3");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC07BF78B0DD");

            entity.ToTable("Classroom");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Equipment).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.RoomName).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC077A949D04");

            entity.HasIndex(e => e.DepartmentId, "IX_Courses_DepartmentID");

            entity.HasIndex(e => e.CourseCode, "UQ__Courses__FC00E000C9449B35").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseCode).HasMaxLength(20);
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__Departm__6EF57B66");

            entity.HasMany(d => d.CorequisiteCourses).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__76969D2E"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__75A278F5"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E6D9E6BBA6");
                        j.ToTable("CourseCorequisites");
                        j.HasIndex(new[] { "CorequisiteCourseId" }, "IX_CourseCorequisites_CorequisiteCourseID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("CorequisiteCourseId").HasColumnName("CorequisiteCourseID");
                    });

            entity.HasMany(d => d.Courses).WithMany(p => p.CorequisiteCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__75A278F5"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__76969D2E"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E6D9E6BBA6");
                        j.ToTable("CourseCorequisites");
                        j.HasIndex(new[] { "CorequisiteCourseId" }, "IX_CourseCorequisites_CorequisiteCourseID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("CorequisiteCourseId").HasColumnName("CorequisiteCourseID");
                    });

            entity.HasMany(d => d.CoursesNavigation).WithMany(p => p.PrerequisiteCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__71D1E811"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__72C60C4A"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD9DABCBD5");
                        j.ToTable("CoursePrerequisites");
                        j.HasIndex(new[] { "PrerequisiteCourseId" }, "IX_CoursePrerequisites_PrerequisiteCourseID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });

            entity.HasMany(d => d.PrerequisiteCourses).WithMany(p => p.CoursesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__72C60C4A"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__71D1E811"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD9DABCBD5");
                        j.ToTable("CoursePrerequisites");
                        j.HasIndex(new[] { "PrerequisiteCourseId" }, "IX_CoursePrerequisites_PrerequisiteCourseID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });
        });

        modelBuilder.Entity<CourseEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEv__3214EC076F94292C");

            entity.HasIndex(e => e.ClassSectionId, "IX_CourseEvaluations_ClassSectionID");

            entity.HasIndex(e => e.StudentId, "IX_CourseEvaluations_StudentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Class__2739D489");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Stude__282DF8C2");
        });

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC07FD84D7CD");

            entity.HasIndex(e => e.ClassSectionId, "IX_CourseRegistrations_ClassSectionID");

            entity.HasIndex(e => e.RegistrationPeriodId, "IX_CourseRegistrations_RegistrationPeriodId");

            entity.HasIndex(e => e.StudentId, "IX_CourseRegistrations_StudentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TuitionStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Class__17036CC0");

            entity.HasOne(d => d.RegistrationPeriod).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.RegistrationPeriodId)
                .HasConstraintName("FK__CourseReg__Regis__4A8310C6");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Stude__160F4887");
        });

        modelBuilder.Entity<CourseSyllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSy__3214EC07A907D901");

            entity.HasIndex(e => e.CourseId, "IX_CourseSyllabi_CourseID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Version).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSyllabi)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSyl__Cours__7B5B524B");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC07BCD57E06");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FinancialAid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC07CD752F94");

            entity.HasIndex(e => e.DepartmentId, "IX_FinancialAids_DepartmentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AidName).HasMaxLength(100);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

            entity.HasOne(d => d.Department).WithMany(p => p.FinancialAids)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Financial__Depar__5EBF139D");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC07EB54789C");

            entity.HasIndex(e => e.CourseRegistrationId, "IX_Grades_CourseRegistrationID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseRegistrationId).HasColumnName("CourseRegistrationID");
            entity.Property(e => e.GradeValue)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.QualityPoints).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.CourseRegistration).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseRegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__CourseRe__1BC821DD");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Majors__3214EC0706A736F8");

            entity.HasIndex(e => e.DepartmentId, "IX_Majors_DepartmentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.MajorName).HasMaxLength(100);

            entity.HasOne(d => d.Department).WithMany(p => p.Majors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Majors__Departme__44FF419A");

            entity.HasMany(d => d.Courses).WithMany(p => p.Majors)
                .UsingEntity<Dictionary<string, object>>(
                    "MajorCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MajorCour__Cours__2BFE89A6"),
                    l => l.HasOne<Major>().WithMany()
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MajorCour__Major__2B0A656D"),
                    j =>
                    {
                        j.HasKey("MajorId", "CourseId").HasName("PK__MajorCou__B92A68A96EB66004");
                        j.ToTable("MajorCourses");
                        j.HasIndex(new[] { "CourseId" }, "IX_MajorCourses_CourseID");
                        j.IndexerProperty<Guid>("MajorId").HasColumnName("MajorID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<MidtermEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MidtermE__3214EC0733774C4A");

            entity.HasIndex(e => e.CourseRegistrationId, "IX_MidtermEvaluations_CourseRegistrationID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseRegistrationId).HasColumnName("CourseRegistrationID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CourseRegistration).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.CourseRegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MidtermEv__Cours__1F98B2C1");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Professo__3214EC07E1AB028E");

            entity.HasIndex(e => e.DepartmentId, "IX_Professors_DepartmentID");

            entity.HasIndex(e => e.UserId, "IX_Professors_UserID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Department).WithMany(p => p.Professors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Professor__Depar__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Professors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Professor__UserI__4D94879B");
        });

        modelBuilder.Entity<RegistrationPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC071C5822B6");

            entity.HasIndex(e => e.CreatedBy, "IX_RegistrationPeriods_CreatedBy");

            entity.HasIndex(e => e.SemesterId, "IX_RegistrationPeriods_SemesterId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RegistrationPeriods)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Creat__3E1D39E1");

            entity.HasOne(d => d.Semester).WithMany(p => p.RegistrationPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Semes__3D2915A8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07D0B34CBE");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160ED56A0DA").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scholars__3214EC074C907D15");

            entity.HasIndex(e => e.DepartmentId, "IX_Scholarships_DepartmentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.ScholarshipName).HasMaxLength(100);

            entity.HasOne(d => d.Department).WithMany(p => p.Scholarships)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Scholarsh__Depar__5AEE82B9");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Semester__3214EC077B57D336");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SemesterName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceR__3214EC079BC47696");

            entity.HasIndex(e => e.StudentId, "IX_ServiceRequests_StudentID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceRe__Stude__236943A5");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC072A641ED3");

            entity.HasIndex(e => e.MajorId, "IX_Students_MajorID");

            entity.HasIndex(e => e.UserId, "IX_Students_UserID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AdmissionStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.Mssv)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MSSV");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__MajorI__49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__UserID__48CFD27E");
        });

        modelBuilder.Entity<StudentFinancialAid>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.FinancialAidId }).HasName("PK__StudentF__2337769D9F585E8C");

            entity.HasIndex(e => e.FinancialAidId, "IX_StudentFinancialAids_FinancialAidID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.FinancialAidId).HasColumnName("FinancialAidID");

            entity.HasOne(d => d.FinancialAid).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.FinancialAidId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentFi__Finan__66603565");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentFi__Stude__656C112C");
        });

        modelBuilder.Entity<StudentScholarship>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ScholarshipId }).HasName("PK__StudentS__EA96C6580B50F8F2");

            entity.HasIndex(e => e.ScholarshipId, "IX_StudentScholarships_ScholarshipID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ScholarshipId).HasColumnName("ScholarshipID");

            entity.HasOne(d => d.Scholarship).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.ScholarshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentSc__Schol__628FA481");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentSc__Stude__619B8048");
        });

        modelBuilder.Entity<StudentTuition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentT__3214EC071197F40B");

            entity.ToTable("StudentTuition");

            entity.HasIndex(e => e.SemesterId, "IX_StudentTuition_SemesterID");

            entity.HasIndex(e => e.StudentId, "IX_StudentTuition_StudentID");

            entity.HasIndex(e => e.TuitionPolicyId, "IX_StudentTuition_TuitionPolicyID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TuitionPolicyId).HasColumnName("TuitionPolicyID");

            entity.HasOne(d => d.Semester).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Semes__04E4BC85");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Stude__03F0984C");

            entity.HasOne(d => d.TuitionPolicy).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.TuitionPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Tuiti__05D8E0BE");
        });

        modelBuilder.Entity<Tuition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tuitions__3214EC070E564B1A");

            entity.HasIndex(e => e.SemesterId, "IX_Tuitions_SemesterId");

            entity.HasIndex(e => e.StudentId, "IX_Tuitions_StudentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.LastPaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Semester).WithMany(p => p.Tuitions)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tuitions__Semest__489AC854");

            entity.HasOne(d => d.Student).WithMany(p => p.Tuitions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tuitions__Studen__47A6A41B");
        });

        modelBuilder.Entity<TuitionPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC07DE141B5E");

            entity.HasIndex(e => e.CreatedBy, "IX_TuitionPeriods_CreatedBy");

            entity.HasIndex(e => e.SemesterId, "IX_TuitionPeriods_SemesterId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TuitionPeriods)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TuitionPe__Creat__42E1EEFE");

            entity.HasOne(d => d.Semester).WithMany(p => p.TuitionPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TuitionPe__Semes__41EDCAC5");
        });

        modelBuilder.Entity<TuitionPolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC0745FBEDBD");

            entity.HasIndex(e => e.MajorId, "IX_TuitionPolicies_MajorID");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.PolicyName).HasMaxLength(100);

            entity.HasOne(d => d.Major).WithMany(p => p.TuitionPolicies)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__TuitionPo__Major__571DF1D5");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07D9E1546D");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534583BA837").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
