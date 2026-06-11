using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISchemaStructureService
    {
        /// <summary>
        /// Returns schema names from TBL_EVAL_SCHEMAMASTER for a given course/regulation/sem.
        /// Inline SQL: SELECT cast(IDENT as varchar(100)) as IDENT, QNO, '' as MAXMRK
        ///             FROM TBL_EVAL_SCHEMAMASTER order by QNO
        /// (filtered by Course, Regulation, Sem when provided)
        /// </summary>
        Task<IEnumerable<object>> GetSchemaListAsync(string course, string regulation, string sem);

        /// <summary>
        /// Checks if a schema name already exists.
        /// SP: Sp_Eval_Check_Schema @SchemaName
        /// Returns: COUNT(*) — 0 = available, >0 = already exists
        /// </summary>
        Task<IEnumerable<object>> CheckSchemaAsync(string schemaName);

        /// <summary>
        /// Loads schema master + structure rows for editing.
        /// SP: SP_EVAL_LOAD_STRUCTURE_Edit @SchemaName
        /// </summary>
        Task<IEnumerable<object>> LoadStructureForEditAsync(string schemaName);

        /// <summary>
        /// Loads schema structure rows for display/view mode.
        /// SP: SP_EVAL_LOAD_STRUCTURE @SchemaName
        /// </summary>
        Task<IEnumerable<object>> LoadStructureAsync(string schemaName);

        /// <summary>
        /// Saves schema master header + all question rows.
        /// Flow: SP_EVAL_SCHEMAMASTER_SAVE (once) → SP_EVAL_SCHEMASTRUCTURE_SAVE (loop per question)
        /// Returns total rows saved by the structure save calls.
        /// </summary>
        Task<int> SaveSchemaStructureAsync(SaveSchemaStructureRequest req);

        /// <summary>
        /// Deletes a schema and all its question rows.
        /// SP: Sp_Eval_Delete_Pap_Data @SchemaName
        /// </summary>
        Task<int> DeleteSchemaAsync(string schemaName);
    }
}
