using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    /// <summary>
    /// Evaluation — Schema Marks Entry (Evaluation/Schema_MarksEntry.aspx)
    /// Class: iCampus_Evaluation_Default
    ///
    /// Page flow:
    ///   1. Enter OMR Number → btnDisplay_Click
    ///      → [SP_EVAL_LOAD_STRUCTURE_MarksEntry] loads schema structure grid
    ///      → SELECT QUES FROM TBL_EVAL_QPSTRUCTURE loads QP question rules (for JS calculations)
    ///      → iframe loads scanned answer booklet images
    ///   2. Enter marks per sub-question in txtStudentMarks textboxes
    ///      → JS Compare() calculates per-question and grand totals
    ///   3. btnSave_Click → Sp_Eval_Insert_Student_SecuredMarks per question
    ///   4. Finalize → UPDATE FStatus='Y' in tbl_Eval_Student_Secured_Marks
    ///
    /// BAL/DAL: BAL_EVAL_MARKSENTRY / DAL_EVAL_MARKSENTRY
    /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    public class SchemaMarksEntryService : ISchemaMarksEntryService
    {
        private readonly IGenericRepository<object> _repo;

        public SchemaMarksEntryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load schema structure for marks entry form.
        /// SP: [SP_EVAL_LOAD_STRUCTURE_MarksEntry] @SchemaName
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 124504
        ///   Called with square brackets: [SP_EVAL_LOAD_STRUCTURE_MarksEntry] 'SchemaName'
        /// </summary>
        public async Task<IEnumerable<object>> LoadStructureForMarksEntryAsync(string schemaName)
        {
            var p1 = new SqlParameter("@SchemaName", schemaName ?? "");
            var sql = StoredProcSql.Exec(StoredProcedures.SP_EVAL_LOAD_STRUCTURE_MarksEntry, "@SchemaName");
            return await _repo.QueryFromStoredProcAsync(sql, p1);
        }

        /// <summary>
        /// Get QP questions text (used by JS for best-of / OR group calculations).
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 124799
        /// </summary>
        public async Task<IEnumerable<object>> GetQPQuestionsAsync(string schemaName)
        {
            var sql = "SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME = @SchemaName";
            var prms = new[] { new SqlParameter("@SchemaName", schemaName ?? "") };
            return await _repo.QueryFromStoredProcAsync(sql, prms);
        }

        /// <summary>
        /// Get optional questions for a schema (for best-of calculation rules).
        /// Inline SQL: SELECT * FROM TBL_EVAL_SCHEMASTRUCTURE
        ///   WHERE COURSE=@Course AND REGULATION=@Regulation
        ///   AND SCHEMANAME=@SchemaName AND QSTATUS='O' ORDER BY QNO
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 125134
        /// </summary>
        public async Task<IEnumerable<object>> GetOptionalQuestionsAsync(string course, string regulation, string schemaName)
        {
            var sql = @"SELECT * FROM TBL_EVAL_SCHEMASTRUCTURE
                        WHERE COURSE = @Course AND REGULATION = @Regulation
                        AND SCHEMANAME = @SchemaName AND QSTATUS = 'O'
                        ORDER BY QNO";
            var prms = new[]
            {
                new SqlParameter("@Course",     course ?? ""),
                new SqlParameter("@Regulation", regulation ?? ""),
                new SqlParameter("@SchemaName", schemaName ?? "")
            };
            return await _repo.QueryFromStoredProcAsync(sql, prms);
        }

        /// <summary>
        /// Get student's previously entered secured marks.
        /// SP: Sp_Eval_Get_SecuredMarks @SchemaName, @PapCode, @OmrNo
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 125134
        /// </summary>
        public async Task<IEnumerable<object>> GetSecuredMarksAsync(string schemaName, string papCode, string omrNo)
        {
            var ps = new[]
            {
                new SqlParameter("@SchemaName", schemaName ?? ""),
                new SqlParameter("@PapCode",    papCode ?? ""),
                new SqlParameter("@OmrNo",      omrNo ?? "")
            };
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Eval_Get_SecuredMarks,
                "@SchemaName", "@PapCode", "@OmrNo");
            return await _repo.QueryFromStoredProcAsync(sql, ps);
        }

        /// <summary>
        /// Save student's secured marks per question.
        /// SP: Sp_Eval_Insert_Student_SecuredMarks (called per question in a loop)
        ///   params: @SchemaName, @PapCode, @OmrNo, @Qno, @SecuredMarks, @SubMarks, @FinalMarks,
        ///           @Sem, @Course, @Regulation, @EvaluatorId
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 124879
        /// </summary>
        public async Task<int> SaveSecuredMarksAsync(SaveSchemaMarksRequest req)
        {
            int totalRows = 0;
            var sqlText = StoredProcSql.Exec(StoredProcedures.Sp_Eval_Insert_Student_SecuredMarks,
                "@SchemaName", "@PapCode", "@OmrNo", "@Qno", "@SecuredMarks",
                "@SubMarks", "@FinalMarks", "@Sem", "@Course", "@Regulation", "@EvaluatorId");

            foreach (var mark in req.Marks)
            {
                var ps = new[]
                {
                    new SqlParameter("@SchemaName",   req.SchemaName ?? ""),
                    new SqlParameter("@PapCode",      req.PapCode ?? ""),
                    new SqlParameter("@OmrNo",        req.OmrNo ?? ""),
                    new SqlParameter("@Qno",          mark.Qno ?? ""),
                    new SqlParameter("@SecuredMarks", mark.SecuredMarks ?? ""),
                    new SqlParameter("@SubMarks",     mark.SubMarks ?? ""),
                    new SqlParameter("@FinalMarks",   mark.FinalMarks ?? ""),
                    new SqlParameter("@Sem",          req.Sem ?? ""),
                    new SqlParameter("@Course",       req.Course ?? ""),
                    new SqlParameter("@Regulation",   req.Regulation ?? ""),
                    new SqlParameter("@EvaluatorId",  req.EvaluatorId ?? "")
                };
                totalRows += await _repo.ExecuteStoredProcAsync(sqlText, ps);
            }
            return totalRows;
        }

        /// <summary>
        /// Finalize marks — set FStatus='Y'.
        /// Inline SQL: UPDATE tbl_Eval_Student_Secured_Marks SET FStatus='Y'
        ///   WHERE SchemaName=@SchemaName AND PapCode=@PapCode
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 125040
        /// </summary>
        public async Task<int> FinalizeMarksAsync(FinalizeMarksRequest req)
        {
            var sql = @"UPDATE tbl_Eval_Student_Secured_Marks
                        SET FStatus = 'Y'
                        WHERE SchemaName = @SchemaName AND PapCode = @PapCode";
            var prms = new[]
            {
                new SqlParameter("@SchemaName", req.SchemaName ?? ""),
                new SqlParameter("@PapCode",    req.PapCode ?? "")
            };
            return await _repo.ExecuteStoredProcAsync(sql, prms);
        }
    }
}
