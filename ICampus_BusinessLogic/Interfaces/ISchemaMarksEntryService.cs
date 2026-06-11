using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    /// <summary>
    /// Evaluation — Schema Marks Entry (Evaluation/Schema_MarksEntry.aspx)
    /// Class: iCampus_Evaluation_Default
    /// BAL/DAL: BAL_EVAL_MARKSENTRY / DAL_EVAL_MARKSENTRY
    /// </summary>
    public interface ISchemaMarksEntryService
    {
        /// <summary>
        /// Load schema structure for marks entry form.
        /// SP: [SP_EVAL_LOAD_STRUCTURE_MarksEntry] @SchemaName
        /// </summary>
        Task<IEnumerable<object>> LoadStructureForMarksEntryAsync(string schemaName);

        /// <summary>
        /// Get QP questions text for marks entry (used by JS for best-of calculations).
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        /// </summary>
        Task<IEnumerable<object>> GetQPQuestionsAsync(string schemaName);

        /// <summary>
        /// Get optional questions for a schema (used for best-of rules).
        /// Inline SQL: SELECT * FROM TBL_EVAL_SCHEMASTRUCTURE
        ///   WHERE COURSE=@Course AND REGULATION=@Regulation AND SCHEMANAME=@SchemaName AND QSTATUS='O'
        ///   ORDER BY QNO
        /// </summary>
        Task<IEnumerable<object>> GetOptionalQuestionsAsync(string course, string regulation, string schemaName);

        /// <summary>
        /// Get student's previously entered secured marks.
        /// SP: Sp_Eval_Get_SecuredMarks @SchemaName, @PapCode, @OmrNo
        /// </summary>
        Task<IEnumerable<object>> GetSecuredMarksAsync(string schemaName, string papCode, string omrNo);

        /// <summary>
        /// Save student's secured marks per question.
        /// SP: Sp_Eval_Insert_Student_SecuredMarks per question
        /// </summary>
        Task<int> SaveSecuredMarksAsync(SaveSchemaMarksRequest req);

        /// <summary>
        /// Finalize marks — set FStatus='Y'.
        /// Inline SQL: UPDATE tbl_Eval_Student_Secured_Marks SET FStatus='Y'
        ///   WHERE SchemaName=@SchemaName AND PapCode=@PapCode
        /// </summary>
        Task<int> FinalizeMarksAsync(FinalizeMarksRequest req);
    }
}
