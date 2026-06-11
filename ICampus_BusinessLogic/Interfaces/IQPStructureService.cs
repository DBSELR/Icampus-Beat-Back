using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    /// <summary>
    /// Evaluation — QP Structure (Question Paper Structure)
    /// Page: Evaluation/QP_Structure.aspx
    /// Class: iCampus_Evaluation_QP_Structure
    /// BAL/DAL: BAL_Eval_QP_Structure / DAL_Eval_QP_Structure
    /// </summary>
    public interface IQPStructureService
    {
        /// <summary>
        /// Get QP structure (compulsory + optional questions) for a schema.
        /// SP: Sp_Eval_Get_Qp_Structure @SchemaName
        /// Returns: QNO, MAXMRK, QSTATUS, MAXNOOFQUESTIONS, MAXSECTIONS from TBL_EVAL_SCHEMASTRUCTURE
        /// </summary>
        Task<IEnumerable<object>> GetQPStructureAsync(string schemaName);

        /// <summary>
        /// Get saved QP questions text from TBL_EVAL_QPSTRUCTURE.
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        /// </summary>
        Task<IEnumerable<object>> GetSavedQuestionsAsync(string schemaName);

        /// <summary>
        /// Save QP structure (question selections).
        /// SP: SP_EVAL_QPSTRUCTURE_SAVE @SchemaName, @Questions, ...
        /// </summary>
        Task<int> SaveQPStructureAsync(SaveQPStructureRequest req);
    }
}
