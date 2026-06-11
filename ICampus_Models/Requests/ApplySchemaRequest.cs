using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for applying a schema to one or more papers.
    /// Mirrors Evaluation/Apply_Schema.aspx flow:
    ///   Sp_Eval_Save_UserPapers is called once per selected paper in a loop.
    /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll UTF-16LE analysis.
    /// </summary>
    public class SaveApplySchemaRequest
    {
        public string SchemaName { get; set; }
        public string Course     { get; set; }
        public string Regulation { get; set; }
        public string Sem        { get; set; }
        public string ExamMY     { get; set; }

        /// <summary>List of PapCodes to apply this schema to.</summary>
        public List<string> PapCodes { get; set; } = new List<string>();
    }
}
