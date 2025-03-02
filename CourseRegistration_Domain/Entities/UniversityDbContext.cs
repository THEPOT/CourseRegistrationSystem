using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistration_Domain.Entities;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext()
    {
    }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEvaluation> CourseEvaluations { get; set; }

    public virtual DbSet<CourseOffering> CourseOfferings { get; set; }

    public virtual DbSet<CourseSyllabus> CourseSyllabi { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<FinancialAid> FinancialAids { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<MidtermEvaluation> MidtermEvaluations { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Scholarship> Scholarships { get; set; }

    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentFinancialAid> StudentFinancialAids { get; set; }

    public virtual DbSet<StudentScholarship> StudentScholarships { get; set; }

    public virtual DbSet<StudentTuition> StudentTuitions { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    public virtual DbSet<TuitionPolicy> TuitionPolicies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=UniversityDb;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC07DDB77859");

            entity.HasIndex(e => e.CourseCode, "UQ__Courses__FC00E00094663EAF").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.LearningOutcomes).IsUnicode(false);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Courses)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__Faculty__7CA47C3F");

            entity.HasMany(d => d.CorequisiteCourses).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__04459E07"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__035179CE"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E69AFA442E");
                        j.ToTable("CourseCorequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("CorequisiteCourseId").HasColumnName("CorequisiteCourseID");
                    });

            entity.HasMany(d => d.Courses).WithMany(p => p.CorequisiteCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__035179CE"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__04459E07"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E69AFA442E");
                        j.ToTable("CourseCorequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("CorequisiteCourseId").HasColumnName("CorequisiteCourseID");
                    });

            entity.HasMany(d => d.CoursesNavigation).WithMany(p => p.PrerequisiteCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__7F80E8EA"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__00750D23"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD4CE9EED0");
                        j.ToTable("CoursePrerequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });

            entity.HasMany(d => d.PrerequisiteCourses).WithMany(p => p.CoursesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__00750D23"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__7F80E8EA"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD4CE9EED0");
                        j.ToTable("CoursePrerequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });
        });

        modelBuilder.Entity<CourseEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEv__3214EC076BCEEDA0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Comments).IsUnicode(false);
            entity.Property(e => e.CourseOfferingId).HasColumnName("CourseOfferingID");
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.CourseOffering).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.CourseOfferingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Cours__320C68B7");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Stude__33008CF0");
        });

        modelBuilder.Entity<CourseOffering>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseOf__3214EC074A611ADE");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Classroom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.LecturerId).HasColumnName("LecturerID");
            entity.Property(e => e.Schedule)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TermId).HasColumnName("TermID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseOfferings)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseOff__Cours__184C96B4");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.CourseOfferings)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseOff__Lectu__1A34DF26");

            entity.HasOne(d => d.Term).WithMany(p => p.CourseOfferings)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseOff__TermI__1940BAED");
        });

        modelBuilder.Entity<CourseSyllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSy__3214EC071A2F0F3E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SyllabusContent).IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Version)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSyllabi)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSyl__Cours__090A5324");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC07FDE21D3B");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34A0258C0D").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facultie__3214EC07C86DB1EF");

            entity.HasIndex(e => e.FacultyName, "UQ__Facultie__BFD889E18FEF3449").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FacultyName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FinancialAid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC0764D0C19D");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AidName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.EligibilityCriteria).IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.FinancialAids)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Financial__Depar__703EA55A");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC07A2B18D8A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.GradeValue)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.QualityPoints).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");

            entity.HasOne(d => d.Registration).WithMany(p => p.Grades)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__Registra__24B26D99");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lecturer__3214EC07F8BAF1E6");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Lecturers)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lecturers__Facul__60083D91");

            entity.HasOne(d => d.User).WithMany(p => p.Lecturers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lecturers__UserI__5F141958");
        });

        modelBuilder.Entity<MidtermEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MidtermE__3214EC07322128F3");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Recommendation).IsUnicode(false);
            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Registration).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MidtermEv__Regis__2882FE7D");
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Programs__3214EC07548DA55A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Programs)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Programs__Facult__567ED357");

            entity.HasMany(d => d.Courses).WithMany(p => p.Programs)
                .UsingEntity<Dictionary<string, object>>(
                    "ProgramCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProgramCo__Cours__36D11DD4"),
                    l => l.HasOne<Program>().WithMany()
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProgramCo__Progr__35DCF99B"),
                    j =>
                    {
                        j.HasKey("ProgramId", "CourseId").HasName("PK__ProgramC__19B7B72002181C44");
                        j.ToTable("ProgramCourses");
                        j.IndexerProperty<Guid>("ProgramId").HasColumnName("ProgramID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC079452151E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseOfferingId).HasColumnName("CourseOfferingID");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.CourseOffering).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.CourseOfferingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Cours__1FEDB87C");

            entity.HasOne(d => d.Student).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Stude__1EF99443");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0790A3A3C6");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616074B8A274").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scholars__3214EC07E8AE0911");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.EligibilityCriteria).IsUnicode(false);
            entity.Property(e => e.ScholarshipName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Scholarships)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Scholarsh__Depar__6C6E1476");
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceR__3214EC07AC0EFCA8");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Details).IsUnicode(false);
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceRe__Stude__2E3BD7D3");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Staff__3214EC0700892601");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Department).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__Departmen__64CCF2AE");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__UserID__63D8CE75");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC07B9CE3A7F");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AdmissionStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Mssv)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MSSV");
            entity.Property(e => e.ProgramId).HasColumnName("ProgramID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Program).WithMany(p => p.Students)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__Progra__5B438874");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__UserID__5A4F643B");
        });

        modelBuilder.Entity<StudentFinancialAid>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.FinancialAidId }).HasName("PK__StudentF__2337769D3D19B812");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.FinancialAidId).HasColumnName("FinancialAidID");

            entity.HasOne(d => d.FinancialAid).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.FinancialAidId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentFi__Finan__77DFC722");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentFi__Stude__76EBA2E9");
        });

        modelBuilder.Entity<StudentScholarship>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ScholarshipId }).HasName("PK__StudentS__EA96C658C0575390");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ScholarshipId).HasColumnName("ScholarshipID");

            entity.HasOne(d => d.Scholarship).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.ScholarshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentSc__Schol__740F363E");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentSc__Stude__731B1205");
        });

        modelBuilder.Entity<StudentTuition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentT__3214EC073F7B5AE4");

            entity.ToTable("StudentTuition");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TermId).HasColumnName("TermID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TuitionPolicyId).HasColumnName("TuitionPolicyID");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Stude__1293BD5E");

            entity.HasOne(d => d.Term).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__TermI__1387E197");

            entity.HasOne(d => d.TuitionPolicy).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.TuitionPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Tuiti__147C05D0");
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Terms__3214EC079B6C9CD5");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TermName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TuitionPolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC071AAABAC9");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.PolicyName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProgramId).HasColumnName("ProgramID");

            entity.HasOne(d => d.Program).WithMany(p => p.TuitionPolicies)
                .HasForeignKey(d => d.ProgramId)
                .HasConstraintName("FK__TuitionPo__Progr__689D8392");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC072932617B");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CF42E5AB").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__52AE4273");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
