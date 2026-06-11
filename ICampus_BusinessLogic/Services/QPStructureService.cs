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
    /// Evaluation — QP Structure (Question Paper Structure)
    /// Page: Evaluation/QP_Structure.aspx
    /// Class: iCampus_Evaluation_QP_Structure
    ///
    /// Page flow:
    ///   Page_Load → Load_SchemaMaster (reuses SchemaStructure schemas endpoint)
    ///   ddlSavedSchema_SelectedIndexChanged → Sp_Eval_Get_Qp_Structure
    ///     Populates lbQNO1 (compulsory, ForeColor=#990000) and lbQNO2 (optional, ForeColor=#6600cc)
    ///   btnAdd_Click → transfers selected questions to txtQuestions
    ///   btnSave_Click → Save_Data → SP_EVAL_QPSTRUCTURE_SAVE
    ///
    /// DAL methods: Get_QPStructure, Save_QP_Structure
    /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    public class QPStructureService : IQPStructureService
    {
        private readonly IGenericRepository<object> _repo;

        public QPStructureService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get QP structure (questions with compulsory/optional status) for a schema.
        /// SP: Sp_Eval_Get_Qp_Structure @SchemaName
        /// Returns: QNO, MAXMRK, QSTATUS, MAXNOOFQUESTIONS, MAXSECTIONS
        ///   from TBL_EVAL_SCHEMASTRUCTURE WHERE QNO LIKE ... AND SCHEMANAME=...
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 127636
        /// </summary>
        public async Task<IEnumerable<object>> GetQPStructureAsync(string schemaName)
        {
            var p1 = new SqlParameter("@SchemaName", schemaName ?? "");
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Eval_Get_Qp_Structure, "@SchemaName");
            return await _repo.QueryFromStoredProcAsync(sql, p1);
        }

        /// <summary>
        /// Get saved QP questions text.
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 124799
        /// </summary>
        public async Task<IEnumerable<object>> GetSavedQuestionsAsync(string schemaName)
        {
            var sql = "SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME = @SchemaName";
            var prms = new[] { new SqlParameter("@SchemaName", schemaName ?? "") };
            return await _repo.QueryFromStoredProcAsync(sql, prms);
        }

        /// <summary>
        /// Save QP structure (question selections).
        /// SP: SP_EVAL_QPSTRUCTURE_SAVE @SchemaName, @Questions, @QuestionsSubOption, @TotalMaxMarks, @TotalEnterMaxMarks
        /// Confirmed from DataAccessLayer.dll UTF-16LE: offset 125800 + BOL property analysis
        /// </summary>
        public async Task<int> SaveQPStructureAsync(SaveQPStructureRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@SchemaName",         req.SchemaName ?? ""),
                new SqlParameter("@Questions",          req.Questions ?? ""),
                new SqlParameter("@QuestionsSubOption", req.QuestionsSubOption ?? ""),
                new SqlParameter("@TotalMaxMarks",      req.TotalMaxMarks ?? ""),
                new SqlParameter("@TotalEnterMaxMarks", req.TotalEnterMaxMarks ?? "")
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SP_EVAL_QPSTRUCTURE_SAVE,
                "@SchemaName", "@Questions", "@QuestionsSubOption", "@TotalMaxMarks", "@TotalEnterMaxMarks");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }
    }
}
