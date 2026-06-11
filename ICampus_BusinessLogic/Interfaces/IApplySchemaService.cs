using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IApplySchemaService
    {
        /// <summary>
        /// Returns semester list for the apply-schema form.
        /// SP: Sp_Eval_Load_Sem @Course, @Regulation, @ExamMY
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY);

        /// <summary>
        /// Returns available papers grid with schema-applied status (SStatus).
        /// SP: Sp_Eval_Load_Papers @Course, @Regulation, @Sem, @ExamMY
        /// Returns: pcode, Pname, SStatus columns
        /// </summary>
        Task<IEnumerable<object>> GetPapersAsync(string course, string regulation, string sem, string examMY);

        /// <summary>
        /// Returns papers already assigned to a schema.
        /// SP: Sp_Eval_Get_PapData @SchemaName, @Course, @Regulation, @Sem
        /// Returns: Id, PapCode, NoofQuestions, MaxSections
        /// Confirmed inline SQL: SELECT ... FROM TBL_EVAL_SCHEMAMASTER INNER JOIN tbl_pap ...
        /// </summary>
        Task<IEnumerable<object>> GetAssignedPapersAsync(string schemaName, string course, string regulation, string sem);

        /// <summary>
        /// Applies a schema to a list of selected papers (loop per paper).
        /// SP: Sp_Eval_Save_UserPapers @SchemaName, @PapCode, @Course, @Regulation, @Sem, @ExamMY
        /// Returns total rows saved.
        /// </summary>
        Task<int> SaveApplySchemaAsync(SaveApplySchemaRequest req);

        /// <summary>
        /// Removes one paper from tbl_Eval_UserPapers.
        /// SP: Sp_Eval_Remove_UserPaper @Regulation, @Course, @ExamMY, @TempCode, @SEM
        /// </summary>
        Task<int> DeleteAppliedPaperAsync(string regulation, string course, string examMY, string tempCode, string sem);
    }
}
