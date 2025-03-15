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

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Scholarship> Scholarships { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentFinancialAid> StudentFinancialAids { get; set; }

    public virtual DbSet<StudentScholarship> StudentScholarships { get; set; }

    public virtual DbSet<StudentTuition> StudentTuitions { get; set; }

    public virtual DbSet<TuitionPolicy> TuitionPolicies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=UniversityDb;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministrativeStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC074ED00E1A");

            entity.ToTable("AdministrativeStaff");

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
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC07298E451C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassroomId).HasColumnName("ClassroomID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);
            entity.Property(e => e.ProfessorId).HasColumnName("ProfessorID");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ClassroomId)
                .HasConstraintName("FK__ClassSect__Class__10566F31");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Cours__0D7A0286");

            entity.HasOne(d => d.Professor).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Profe__0F624AF8");

            entity.HasOne(d => d.Semester).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Semes__0E6E26BF");
        });

        modelBuilder.Entity<ClassSectionSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC07F01EF328");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ClassSectionSchedules)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Class__151B244E");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC072EE467B2");

            entity.ToTable("Classroom");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Equipment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC07B90CF3FA");

            entity.HasIndex(e => e.CourseCode, "UQ__Courses__FC00E000F4EE7E49").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.LearningOutcomes).IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__Departm__70DDC3D8");

            entity.HasMany(d => d.CorequisiteCourses).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__787EE5A0"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__778AC167"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E61063E3E0");
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
                        .HasConstraintName("FK__CourseCor__Cours__778AC167"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__787EE5A0"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E61063E3E0");
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
                        .HasConstraintName("FK__CoursePre__Cours__73BA3083"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__74AE54BC"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD5DFF0859");
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
                        .HasConstraintName("FK__CoursePre__Prere__74AE54BC"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__73BA3083"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CD5DFF0859");
                        j.ToTable("CoursePrerequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });
        });

        modelBuilder.Entity<CourseEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEv__3214EC07BC8F95AD");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.Comments).IsUnicode(false);
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Class__2CF2ADDF");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseEva__Stude__2DE6D218");
        });

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC07FD4CD4D4");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Class__1AD3FDA4");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Stude__19DFD96B");
        });

        modelBuilder.Entity<CourseSyllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSy__3214EC0756B75525");

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
                .HasConstraintName("FK__CourseSyl__Cours__7D439ABD");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC07731F1E59");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC346CF3D5A4").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FinancialAid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC07B5FAB6EB");

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
                .HasConstraintName("FK__Financial__Depar__5EBF139D");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC07DA914A10");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseRegistrationId).HasColumnName("CourseRegistrationID");
            entity.Property(e => e.GradeValue)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.QualityPoints).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.CourseRegistration).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseRegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__CourseRe__1F98B2C1");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Majors__3214EC07EC658630");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.MajorName)
                .HasMaxLength(100)
                .IsUnicode(false);

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
                        .HasConstraintName("FK__MajorCour__Cours__31B762FC"),
                    l => l.HasOne<Major>().WithMany()
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MajorCour__Major__30C33EC3"),
                    j =>
                    {
                        j.HasKey("MajorId", "CourseId").HasName("PK__MajorCou__B92A68A906F8AFFF");
                        j.ToTable("MajorCourses");
                        j.IndexerProperty<Guid>("MajorId").HasColumnName("MajorID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<MidtermEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MidtermE__3214EC077F162F23");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseRegistrationId).HasColumnName("CourseRegistrationID");
            entity.Property(e => e.Recommendation).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CourseRegistration).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.CourseRegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MidtermEv__Cours__236943A5");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Professo__3214EC0750B62E20");

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07BC7755F6");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160EDA843F0").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scholars__3214EC07F3071F8E");

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
                .HasConstraintName("FK__Scholarsh__Depar__5AEE82B9");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Semester__3214EC0736A66FAD");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SemesterName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceR__3214EC07F942E316");

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
                .HasConstraintName("FK__ServiceRe__Stude__29221CFB");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC0780BC73C6");

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
            entity.HasKey(e => new { e.StudentId, e.FinancialAidId }).HasName("PK__StudentF__2337769D58920B1A");

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
            entity.HasKey(e => new { e.StudentId, e.ScholarshipId }).HasName("PK__StudentS__EA96C65814E78592");

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
            entity.HasKey(e => e.Id).HasName("PK__StudentT__3214EC0715CCA30D");

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
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TuitionPolicyId).HasColumnName("TuitionPolicyID");

            entity.HasOne(d => d.Semester).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Semes__07C12930");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Stude__06CD04F7");

            entity.HasOne(d => d.TuitionPolicy).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.TuitionPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentTu__Tuiti__08B54D69");
        });

        modelBuilder.Entity<TuitionPolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC079861815C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.PolicyName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Major).WithMany(p => p.TuitionPolicies)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__TuitionPo__Major__571DF1D5");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07257C95EB");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342EAFC01C").IsUnique();

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
                .HasConstraintName("FK__Users__RoleId__412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
