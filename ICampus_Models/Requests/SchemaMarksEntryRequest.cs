using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for saving student secured marks against a schema.
    /// Maps to Sp_Eval_Insert_Student_SecuredMarks.
    ///
    /// Page: Evaluation/Schema_MarksEntry.aspx
    /// Flow:
    ///   1. Enter OMR Number → Display → loads answer script + schema structure
    ///   2. Enter marks per sub-question → JS calculates totals
    ///   3. Save → Sp_Eval_Insert_Student_SecuredMarks per question
    ///
    /// BOL: BOL_EVAL_MARKSENTRY — uses SchemaName, PapCode, OmrNo, marks data
    /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    public class SaveSchemaMarksRequest
    {
        public string SchemaName { get; set; }
        public string PapCode   { get; set; }
        public string OmrNo     { get; set; }
        public string Sem       { get; set; }
        public string Course    { get; set; }
        public string Regulation { get; set; }
        public string EvaluatorId { get; set; }

        /// <summary>One entry per question with marks.</summary>
        public List<QuestionMarkEntry> Marks { get; set; } = new List<QuestionMarkEntry>();
    }

    /// <summary>
    /// One question's secured marks entry.
    /// </summary>
    public class QuestionMarkEntry
    {
        public string Qno           { get; set; }
        public string SecuredMarks  { get; set; }
        public string SubMarks      { get; set; }
        public string FinalMarks    { get; set; }
    }

    /// <summary>
    /// Request for finalizing marks (set FStatus='Y').
    /// </summary>
    public class FinalizeMarksRequest
    {
        public string SchemaName { get; set; }
        public string PapCode   { get; set; }
    }
}
