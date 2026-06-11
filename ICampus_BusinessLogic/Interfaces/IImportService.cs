using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IImportService
    {
        Task<IEnumerable<string>> GetExcelSheetsAsync(IFormFile file);
        Task<System.Data.DataTable> BuildDataTableFromIFormFileAsync(IFormFile file, string sheetName, bool hasHeader = true);

        Task<int> ImportStudentDataAsync(ImportStudentRequest request); // row-by-row calling sp_stdDATA_Importing
        Task<int> ImportElectiveDataAsync(ImportTvTableRequest request); // TVP -> SPM_ELECTIVEDATA_IMPORTING
        Task<int> ImportExamSessionDatesAsync(ImportTvTableRequest request); // TVP -> SP_Import_Exam_Session_Date
        Task<int> ImportCondonationFeeAsync(ImportTvTableRequest request); // TVP -> SP_IMPORT_CONDONATION_FEE
        Task<int> ImportRegistrationBlockAsync(ImportTvTableRequest request); // TVP -> Proc_Import_Registration_block
        Task<int> ImportHallticketsBlockAsync(ImportTvTableRequest request); // TVP -> Proc_Import_Halltickets_block
        Task<int> ImportResultBlockAsync(ImportTvTableRequest request); // TVP -> Proc_Import_Result_block
        Task<int> ImportInternalMarksAsync(ImportTvTableRequest request); // TVP -> Proc_Import_Internal_ImproveMarks
        Task<int> ImportAuditCourseDataAsync(ImportTvTableRequest request); // TVP -> Proc_Import_AuditCourse_Data
        Task<int> ImportRCRVMarksAsync(ImportTvTableRequest request); // TVP -> Proc_Import_RCRV_Marks

        Task<System.Data.DataTable> LoadExcelFormatExtraColumnsAsync(string tableName, string userId);
        Task<IEnumerable<object>> LoadCourseListAsync(string regulation);
        Task<IEnumerable<object>> LoadReguAsync(string course, string regulation, string examMy);
    }
}
