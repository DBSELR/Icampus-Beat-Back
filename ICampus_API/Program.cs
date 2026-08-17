using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ICampus_BusinessLogic.Interfaces;
using ICampus_BusinessLogic.Services;
using ICampus_DataAccessLayer.Data;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
//     DB CONNECTION
// =========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ICampusDbContext>(options =>
    options.UseSqlServer(connectionString));

// =========================
//         CORS
// =========================
var corsSettings = builder.Configuration.GetSection("CorsSettings");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { 
        "http://localhost:3000", 
        "http://localhost:3001",
        "http://localhost:3002",
        "https://localhost:44396",
        "http://localhost:48424",
        "https://icampus.dbasesolutions.in",
        "http://icampus.dbasesolutions.in"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =========================
//       JWT Auth
// =========================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// =========================
//   Services & Repositories
// =========================
builder.Services.AddScoped<IStoredProcExecutor, StoredProcExecutor>();
builder.Services.AddScoped<IGenericRepository<object>, GenericObjectRepositoryAdapter>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICgpService, CgpService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IPaperService, PaperService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<ISubjectGradeService, SubjectGradeService>();
builder.Services.AddScoped<ISemesterGradeService, SemesterGradeService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IFeeHeadService, FeeHeadService>();
builder.Services.AddScoped<IClassGradeService, ClassGradeService>();
builder.Services.AddScoped<IStudentMasterService, StudentMasterService>();
builder.Services.AddScoped<IBranchPriorityMasterService, BranchPriorityMasterService>();
builder.Services.AddScoped<ICourseReportService, CourseReportService>();
builder.Services.AddScoped<ICourseGradeReportService, CourseGradeReportService>();
builder.Services.AddScoped<ISemesterGradeReportService, SemesterGradeReportService>();
builder.Services.AddScoped<IRoomMasterReportService, RoomMasterReportService>();
builder.Services.AddScoped<IExamRegistrationService, ExamRegistrationService>();
builder.Services.AddScoped<ITimeTableAndSeatingService, TimeTableAndSeatingService>();
builder.Services.AddScoped<IRevaluationService, RevaluationService>();
builder.Services.AddScoped<IStudentScreenService, StudentScreenService>();
builder.Services.AddScoped<IMiscFeeService, MiscFeeService>();
builder.Services.AddScoped<IDupCertificateService, DupCertificateService>();
builder.Services.AddScoped<IExamFeeConcessionService, ExamFeeConcessionService>();
builder.Services.AddScoped<IRoomAllotmentService, RoomAllotmentService>();
builder.Services.AddScoped<ICondonationService, CondonationService>();
        builder.Services.AddScoped<IReceiptService, ReceiptService>();
        builder.Services.AddScoped<ICancelReceiptService, CancelReceiptService>();
        builder.Services.AddScoped<IConsolidatedFeeReportService, ConsolidatedFeeReportService>();
        builder.Services.AddScoped<ISupplyLabRegisteredService, SupplyLabRegisteredService>();
        builder.Services.AddScoped<ICreditsMismatchService, CreditsMismatchService>();
        builder.Services.AddScoped<IExamUnRegistrationService, ExamUnRegistrationService>();
        builder.Services.AddScoped<IUnblockRegistrationsService, UnblockRegistrationsService>();
        builder.Services.AddScoped<ITimeTableService, TimeTableService>();
        builder.Services.AddScoped<IHallTicketService, HallTicketService>();
        builder.Services.AddScoped<IQPStatementService, QPStatementService>();
        builder.Services.AddScoped<IOMRSheetService, OMRSheetService>();
        builder.Services.AddScoped<INominalRollsService, NominalRollsService>();
        builder.Services.AddScoped<IExamFeeCollectionService, ExamFeeCollectionService>();
        builder.Services.AddScoped<ICancelReceiptListService, CancelReceiptListService>();
        builder.Services.AddScoped<ISeatingArrangementService, SeatingArrangementService>();
        builder.Services.AddScoped<IRoomAbstractService, RoomAbstractService>();
        builder.Services.AddScoped<IMidHallTicketService, MidHallTicketService>();
        builder.Services.AddScoped<IRoomWiseNominalRollsService, RoomWiseNominalRollsService>();
        builder.Services.AddScoped<IInternalMarksService, InternalMarksService>();
        builder.Services.AddScoped<IPracticalMarksService, PracticalMarksService>();
        builder.Services.AddScoped<IAbsenteesEntryService, AbsenteesEntryService>();
        builder.Services.AddScoped<IRegNoMarksEntryService, RegNoMarksEntryService>();
        builder.Services.AddScoped<IOmrMarksEntryService, OmrMarksEntryService>();
        builder.Services.AddScoped<IExamMyUpdateService, ExamMyUpdateService>();
        builder.Services.AddScoped<IRegnoExammywiseSubjectsService, RegnoExammywiseSubjectsService>();
        builder.Services.AddScoped<IMidAbsenteesEntryService, MidAbsenteesEntryService>();
        builder.Services.AddScoped<IInternalChecklistService, InternalChecklistService>();
        builder.Services.AddScoped<IPracticalChecklistService, PracticalChecklistService>();
        builder.Services.AddScoped<IDFormService, DFormService>();
        builder.Services.AddScoped<IAbsenteesListService, AbsenteesListService>();
        builder.Services.AddScoped<ILabAbsenteesListService, LabAbsenteesListService>();
        builder.Services.AddScoped<IStudentWisePresentListService, StudentWisePresentListService>();
        builder.Services.AddScoped<IDFormMidService, DFormMidService>();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<ISchemaStructureService, SchemaStructureService>();
        builder.Services.AddScoped<IApplySchemaService, ApplySchemaService>();
        builder.Services.AddScoped<IEvaluatorRegistrationService, EvaluatorRegistrationService>();
        builder.Services.AddScoped<IScriptsAssignService, ScriptsAssignService>();
        builder.Services.AddScoped<IPendingListService, PendingListService>();
        builder.Services.AddScoped<IGroftingService, GroftingService>();
        builder.Services.AddScoped<IResultProcessService, ResultProcessService>();
        builder.Services.AddScoped<IModerationService, ModerationService>();
        builder.Services.AddScoped<IStudentHistoryService, StudentHistoryService>();
        builder.Services.AddScoped<IReAdmissionService, ReAdmissionService>();
        builder.Services.AddScoped<IBackLogsListService, BackLogsListService>();
        builder.Services.AddScoped<ITopperService, TopperService>();
        builder.Services.AddScoped<IStudentResultService, StudentResultService>();
        builder.Services.AddScoped<ISubjectwiseFailedListService, SubjectwiseFailedListService>();
        builder.Services.AddScoped<IRvClosingDatesService, RvClosingDatesService>();
        builder.Services.AddScoped<IRegnoWiseSgpaCgpaService, RegnoWiseSgpaCgpaService>();
        builder.Services.AddScoped<ICgpaYearWiseService, CgpaYearWiseService>();
        builder.Services.AddScoped<IMarksDataIntExtService, MarksDataIntExtService>();
        builder.Services.AddScoped<ICreditSecuredService, CreditSecuredService>();
        builder.Services.AddScoped<IOmrNumberUpdateService, OmrNumberUpdateService>();
        builder.Services.AddScoped<IExcelGallyService, ExcelGallyService>();
        builder.Services.AddScoped<IOdDataService, OdDataService>();
        builder.Services.AddScoped<IUniversityDataService, UniversityDataService>();
        builder.Services.AddScoped<ITandPDataService, TandPDataService>();
        builder.Services.AddScoped<IGraceDataService, GraceDataService>();
        builder.Services.AddScoped<IV3DataService, V3DataService>();
        builder.Services.AddScoped<IPreModerationService, PreModerationService>();
        builder.Services.AddScoped<ICoursePercentageService, CoursePercentageService>();
        builder.Services.AddScoped<IBranchwiseCoursePercentService, BranchwiseCoursePercentService>();
        builder.Services.AddScoped<IBranchWisePercentService, BranchWisePercentService>();
        builder.Services.AddScoped<IPassedResultService, PassedResultService>();
        builder.Services.AddScoped<IFailedInResultPassedInSubService, FailedInResultPassedInSubService>();
        builder.Services.AddScoped<IResultGradeSheetService, ResultGradeSheetService>();
        builder.Services.AddScoped<IMtechCmmService, MtechCmmService>();
        builder.Services.AddScoped<IBtechCmmService, BtechCmmService>();
        builder.Services.AddScoped<IMbaCmmService, MbaCmmService>();
        builder.Services.AddScoped<IPcAllCoursesService, PcAllCoursesService>();
        builder.Services.AddScoped<IResultCheckListService, ResultCheckListService>();
        builder.Services.AddScoped<IResultSheetService, ResultSheetService>();
        builder.Services.AddScoped<IResultSheetGraftingService, ResultSheetGraftingService>();
        builder.Services.AddScoped<IResultSheetModerationService, ResultSheetModerationService>();
        builder.Services.AddScoped<IRvMarksCheckListService, RvMarksCheckListService>();
        builder.Services.AddScoped<ITabulationRegisterService, TabulationRegisterService>();
        builder.Services.AddScoped<IRvReportsService, RvReportsService>();
        builder.Services.AddScoped<ISgpaCgpaHtnoWiseService, SgpaCgpaHtnoWiseService>();
        builder.Services.AddScoped<IAwardofClassService, AwardofClassService>();
        builder.Services.AddScoped<IAwardDegreeService, AwardDegreeService>();
        builder.Services.AddScoped<IBranchwiseCourseSecPercentService, BranchwiseCourseSecPercentService>();
        builder.Services.AddScoped<IBranchWiseSectionPercentService, BranchWiseSectionPercentService>();
        builder.Services.AddScoped<ISubjectWiseGradesCountService, SubjectWiseGradesCountService>();
        builder.Services.AddScoped<ITranscriptService, TranscriptService>();
        builder.Services.AddScoped<IIndividualMarksMemoService, IndividualMarksMemoService>();
        builder.Services.AddScoped<ITcIssueService, TcIssueService>();
        builder.Services.AddScoped<IScIssueService, ScIssueService>();
        builder.Services.AddScoped<IUniversityPcFormateService, UniversityPcFormateService>();
        builder.Services.AddScoped<IRvSummeryReportService, RvSummeryReportService>();
        builder.Services.AddScoped<IUniversitySubjectDataService, UniversitySubjectDataService>();
        builder.Services.AddScoped<IUniversityCdDataService, UniversityCdDataService>();
        builder.Services.AddScoped<IAuditCourseService, AuditCourseService>();

// =========================
//   Controllers + Global Auth
// =========================
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddEndpointsApiExplorer();

// =========================
//       Swagger
// =========================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ICampus API",
        Version = "v1"
    });

    // JWT in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token.\nExample: **Bearer eyJhbGciOi...**"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.SupportNonNullableReferenceTypes();
    c.OperationFilter<FileUploadOperationFilter>();
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    c.MapType<IFormFileCollection>(() =>
        new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" } });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// =========================
//   Middleware Pipeline
// =========================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ICampus API v1");
    c.RoutePrefix = "swagger";
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

// 🔥 CORS MUST BE BEFORE Auth & MapControllers
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();