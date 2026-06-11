using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for assigning scripts (answer booklets) to an evaluator.
    /// Mirrors Evaluation/Scripts_Assign.aspx btnSave_Click flow.
    ///
    /// Flow: SP_EVAL_SAVE_SCRIPTS assigns each selected script to the evaluator
    ///       and records QP/Key file paths on the server.
    ///
    /// File uploads (QP PDF + Key PDF) are handled separately via IFormFile
    /// at POST /api/scriptsassign/save using multipart/form-data.
    /// Confirmed from App_Web_xplim0cm.dll ASPX source analysis.
    /// </summary>
    public class SaveScriptsAssignRequest
    {
        /// <summary>Evaluator's USERID from ddlEvaluator</summary>
        public string EvaluatorId { get; set; }

        /// <summary>Paper code from ddlSubject</summary>
        public string PapCode     { get; set; }

        /// <summary>Semester from ddlsem</summary>
        public string Sem         { get; set; }

        /// <summary>Evaluation date from txtEvalDate</summary>
        public string EvalDate    { get; set; }

        /// <summary>Selected bundle numbers from lstBundleNo</summary>
        public List<string> BundleNos { get; set; } = new List<string>();

        /// <summary>Selected script IDs from lstScripts</summary>
        public List<string> ScriptIds { get; set; } = new List<string>();

        /// <summary>
        /// Server-side relative path of uploaded Question Paper PDF.
        /// Populated after file is saved to Evaluation/QuestionPaper/ folder.
        /// </summary>
        public string QpPath  { get; set; }

        /// <summary>
        /// Server-side relative path of uploaded Key PDF.
        /// Populated after file is saved to Evaluation/Key/ folder.
        /// </summary>
        public string KeyPath { get; set; }
    }
}
