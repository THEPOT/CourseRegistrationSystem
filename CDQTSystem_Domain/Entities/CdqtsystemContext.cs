using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_Domain.Entities;

public partial class CdqtsystemContext : DbContext
{
    public CdqtsystemContext()
    {
    }

    public CdqtsystemContext(DbContextOptions<CdqtsystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdministrativeStaff> AdministrativeStaffs { get; set; }

    public virtual DbSet<ClassSection> ClassSections { get; set; }

    public virtual DbSet<ClassSectionSchedule> ClassSectionSchedules { get; set; }

    public virtual DbSet<ClassSession> ClassSessions { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEvaluation> CourseEvaluations { get; set; }

    public virtual DbSet<CourseEvaluationPeriod> CourseEvaluationPeriods { get; set; }

    public virtual DbSet<CourseRegistration> CourseRegistrations { get; set; }

    public virtual DbSet<CourseSyllabus> CourseSyllabi { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<EvaluationAnswer> EvaluationAnswers { get; set; }

    public virtual DbSet<EvaluationOption> EvaluationOptions { get; set; }

    public virtual DbSet<EvaluationQuestion> EvaluationQuestions { get; set; }

    public virtual DbSet<FinancialAid> FinancialAids { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<MidtermEvaluation> MidtermEvaluations { get; set; }

    public virtual DbSet<MidtermEvaluationPeriod> MidtermEvaluationPeriods { get; set; }

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

    public virtual DbSet<TuitionPeriod> TuitionPeriods { get; set; }

    public virtual DbSet<TuitionPolicy> TuitionPolicies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=CDQTSystem;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministrativeStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC07563CD6CA");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Department).WithMany(p => p.AdministrativeStaffs)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdministrativeStaffs_Departments");

            entity.HasOne(d => d.User).WithMany(p => p.AdministrativeStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdministrativeStaffs_Users");
        });

        modelBuilder.Entity<ClassSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC07E24AA81F");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ClassroomId)
                .HasConstraintName("FK_ClassSections_Classrooms");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSections_Courses");

            entity.HasOne(d => d.Professor).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("FK_ClassSections_Professors");

            entity.HasOne(d => d.Semester).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSections_Semesters");
        });

        modelBuilder.Entity<ClassSectionSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSec__3214EC0707A906E0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClassSectionId).HasColumnName("ClassSectionID");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ClassSectionSchedules)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSect__Class__1293BD5E");
        });

        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSes__3214EC07BD1A3C04");

            entity.ToTable("ClassSession");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DayOfWeek).HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ClassSessions)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassSess__Class__72E607DB");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC0794589D18");

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
                .IsUnicode(false);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC07A5ADC457");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CourseCode).HasMaxLength(50);
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.LearningOutcomes).IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Departments");

            entity.HasMany(d => d.CorequisiteCourses).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCorequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__7BB05806"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Cours__7ABC33CD"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E6BA1D5C80");
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
                        .HasConstraintName("FK__CourseCor__Cours__7ABC33CD"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CorequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseCor__Coreq__7BB05806"),
                    j =>
                    {
                        j.HasKey("CourseId", "CorequisiteCourseId").HasName("PK__CourseCo__BE20E9E6BA1D5C80");
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
                        .HasConstraintName("FK__CoursePre__Cours__76EBA2E9"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Prere__77DFC722"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CDE2F8DB79");
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
                        .HasConstraintName("FK__CoursePre__Prere__77DFC722"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CoursePre__Cours__76EBA2E9"),
                    j =>
                    {
                        j.HasKey("CourseId", "PrerequisiteCourseId").HasName("PK__CoursePr__E09340CDE2F8DB79");
                        j.ToTable("CoursePrerequisites");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<Guid>("PrerequisiteCourseId").HasColumnName("PrerequisiteCourseID");
                    });
        });

        modelBuilder.Entity<CourseEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEv__3214EC07980D72E0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.OverallSatisfaction).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEvaluations_Courses");

            entity.HasOne(d => d.Professor).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEvaluations_Professors");

            entity.HasOne(d => d.Semester).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEvaluations_Semesters");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEvaluations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEvaluations_Students");
        });

        modelBuilder.Entity<CourseEvaluationPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEv__3214EC07E14F74AA");

            entity.ToTable("CourseEvaluationPeriod");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Semester).WithMany(p => p.CourseEvaluationPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRegistrations_Semesters");
        });

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC077EF39D84");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TuitionStatus).HasMaxLength(50);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRegistrations_ClassSections");

            entity.HasOne(d => d.RegistrationPeriod).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.RegistrationPeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRegistrations_RegistrationPeriods");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRegistrations_Students");
        });

        modelBuilder.Entity<CourseSyllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSy__3214EC07FE8375FF");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Version).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSyllabi)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseSyllabi_Courses");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC074442D5AA");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepartmentName).HasMaxLength(255);
        });

        modelBuilder.Entity<EvaluationAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evaluati__3214EC0725CA435D");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Evaluation).WithMany(p => p.EvaluationAnswers)
                .HasForeignKey(d => d.EvaluationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EvaluationAnswers_CourseEvaluations");

            entity.HasOne(d => d.Question).WithMany(p => p.EvaluationAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EvaluationAnswers_EvaluationQuestions");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.EvaluationAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .HasConstraintName("FK_EvaluationAnswers_EvaluationOptions");
        });

        modelBuilder.Entity<EvaluationOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evaluati__3214EC07653C88F6");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.OptionText).HasMaxLength(200);
            entity.Property(e => e.OptionTextLocalized).HasMaxLength(200);

            entity.HasOne(d => d.Question).WithMany(p => p.EvaluationOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EvaluationOptions_EvaluationQuestions");
        });

        modelBuilder.Entity<EvaluationQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evaluati__3214EC074B782535");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.QuestionText).HasMaxLength(500);
            entity.Property(e => e.QuestionTextLocalized).HasMaxLength(500);
            entity.Property(e => e.QuestionType).HasMaxLength(50);
        });

        modelBuilder.Entity<FinancialAid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC07B35A9196");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AidName).HasMaxLength(255);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Department).WithMany(p => p.FinancialAids)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_FinancialAids_Departments");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC07B16AD07A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.GradeValue).HasMaxLength(10);
            entity.Property(e => e.QualityPoints).HasColumnType("decimal(3, 2)");

            entity.HasOne(d => d.CourseRegistration).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseRegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_CourseRegistrations");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Majors__3214EC073EFFE966");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.MajorName).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Majors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Majors_Departments");

            entity.HasMany(d => d.Courses).WithMany(p => p.Majors)
                .UsingEntity<Dictionary<string, object>>(
                    "MajorCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MajorCour__Cours__740F363E"),
                    l => l.HasOne<Major>().WithMany()
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MajorCour__Major__731B1205"),
                    j =>
                    {
                        j.HasKey("MajorId", "CourseId").HasName("PK__MajorCou__B92A68A98CD7FC55");
                        j.ToTable("MajorCourses");
                        j.IndexerProperty<Guid>("MajorId").HasColumnName("MajorID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<MidtermEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MidtermE__3214EC07E6E46F78");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_ClassSections");

            entity.HasOne(d => d.Course).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_Courses");

            entity.HasOne(d => d.Professor).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_Professors");

            entity.HasOne(d => d.Semester).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_Semesters");

            entity.HasOne(d => d.Student).WithMany(p => p.MidtermEvaluations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_Students");
        });

        modelBuilder.Entity<MidtermEvaluationPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MidtermE__3214EC07417691D1");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.MidtermEvaluationPeriodNavigation).WithMany(p => p.InverseMidtermEvaluationPeriodNavigation)
                .HasForeignKey(d => d.MidtermEvaluationPeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluations_MidtermEvaluationPeriods");

            entity.HasOne(d => d.Semester).WithMany(p => p.MidtermEvaluationPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MidtermEvaluationPeriods_Semesters");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Professo__3214EC072DD8365C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Department).WithMany(p => p.Professors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Professors_Departments");

            entity.HasOne(d => d.User).WithMany(p => p.Professors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Professors_Users");
        });

        modelBuilder.Entity<RegistrationPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC071E996EC6");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RegistrationPeriods)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegistrationPeriods_Users");

            entity.HasOne(d => d.Semester).WithMany(p => p.RegistrationPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegistrationPeriods_Semesters");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F36F6B48");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scholars__3214EC072B01936E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScholarshipName).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Scholarships)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Scholarships_Departments");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Semester__3214EC07606860D0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AcademicYear).HasMaxLength(50);
            entity.Property(e => e.SemesterName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceR__3214EC0794CF2223");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceType).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Student).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceRequests_Students");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC07B0F57FB7");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AdmissionStatus).HasMaxLength(50);

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Majors");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<StudentFinancialAid>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.FinancialAidId }).HasName("PK__StudentF__2337777F5BA1167F");

            entity.HasOne(d => d.FinancialAid).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.FinancialAidId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentFinancialAids_FinancialAids");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentFinancialAids)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentFinancialAids_Students");
        });

        modelBuilder.Entity<StudentScholarship>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ScholarshipId }).HasName("PK__StudentS__EA96C7B6E1847BB1");

            entity.HasOne(d => d.Scholarship).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.ScholarshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentScholarships_Scholarships");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentScholarships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentScholarships_Students");
        });

        modelBuilder.Entity<StudentTuition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentT__3214EC07356B8424");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Semester).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentTuitions_Semesters");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentTuitions_Students");

            entity.HasOne(d => d.TuitionPolicy).WithMany(p => p.StudentTuitions)
                .HasForeignKey(d => d.TuitionPolicyId)
                .HasConstraintName("FK_StudentTuitions_TuitionPolicies");
        });

        modelBuilder.Entity<TuitionPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC07A050A553");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TuitionPeriods)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TuitionPeriods_Users");

            entity.HasOne(d => d.Semester).WithMany(p => p.TuitionPeriods)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TuitionPeriods_Semesters");
        });

        modelBuilder.Entity<TuitionPolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TuitionP__3214EC07C238D57B");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PolicyName).HasMaxLength(255);

            entity.HasOne(d => d.Major).WithMany(p => p.TuitionPolicies)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_TuitionPolicies_Majors");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07104B714B");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UserCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
