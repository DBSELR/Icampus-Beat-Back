using Microsoft.EntityFrameworkCore;
using ICampus_Models.DTOs;

namespace ICampus_DataAccessLayer.Data
{
    public class ICampusDbContext : DbContext
    {
        public ICampusDbContext(DbContextOptions<ICampusDbContext> options)
            : base(options) { }

        // Keyless DTO used for FromSqlRaw results from PROC_LOAD_REGU_BATCH_COURSE
        public DbSet<CourseDto> CourseDtos => Set<CourseDto>();
        public DbSet<LoginDto> LoginDtos => Set<LoginDto>();
        public DbSet<RegulationDto> RegulationDtos => Set<RegulationDto>();
        public DbSet<CourseListDto> CourseListDtos => Set<CourseListDto>();
        public DbSet<ExamMYDto> ExamMYDtos => Set<ExamMYDto>();
        public DbSet<BatchDto> BatchDtos => Set<BatchDto>();
        public DbSet<BranchDto> BranchDtos => Set<BranchDto>();
        public DbSet<SemDto> SemDtos => Set<SemDto>();
        public DbSet<StreamDto> StreamDtos => Set<StreamDto>();
        public DbSet<PaperListDto> PaperListDtos => Set<PaperListDto>();
        public DbSet<PaperDetailDto> PaperDetailDtos => Set<PaperDetailDto>();
        public DbSet<ExamDto> examDtos => Set<ExamDto>();
        public DbSet<RegCheckDto> regCheckDtos => Set<RegCheckDto>();
        public DbSet<ExamMasterDto> examMasterDtos => Set<ExamMasterDto>();
        public DbSet<ExistingExamDto> existingExamDtos => Set<ExistingExamDto>();
        public DbSet<ExamNotificationDto> examNotificationDtos => Set<ExamNotificationDto>();

        public DbSet<StudentDto> studentDtos => Set<StudentDto>();
        public DbSet<FeeStructureRowDto> feeStructureRowDtos => Set<FeeStructureRowDto>();
        public DbSet<SupplyFeeRowDto> supplyFeeRowDtos => Set<SupplyFeeRowDto>();

        public DbSet<FineRowDto> fineRowDtos => Set<FineRowDto>();
        public DbSet<FeeSemResultDto> feeSemResultDtos => Set<FeeSemResultDto>();
        public DbSet<FeeBranchResultDto> feeBranchResultDtos => Set<FeeBranchResultDto>();
        public DbSet<FeeResupplySemDto> feeResupplySemDtos => Set<FeeResupplySemDto>();
        public DbSet<EmployeeDto> employeeDtos => Set<EmployeeDto>();
        public DbSet<UserGroupDto> UserGroupDtos => Set<UserGroupDto>();

        public DbSet<StudentListDto> studentListDtos => Set<StudentListDto>();
        public DbSet<EmpCheckDto> empCheckDtos => Set<EmpCheckDto>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<LoginDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<RegulationDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<CourseListDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExamMYDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<BatchDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<BranchDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<SemDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<StreamDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<PaperListDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<PaperDetailDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExamDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<RegCheckDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<StudentDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<EmployeeDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<UserGroupDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<StudentListDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<EmpCheckDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExamMasterDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExistingExamDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<FeeStructureRowDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<SupplyFeeRowDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<FineRowDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<FeeSemResultDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<FeeBranchResultDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<FeeResupplySemDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExamNotificationDto>().HasNoKey().ToView(null);



        }
    }
}
