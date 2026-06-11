using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for saving an evaluation schema master + question structure.
    /// Mirrors Evaluation/Schema_Structure.aspx BOL_Eval_Structure flow:
    ///   1. SP_EVAL_SCHEMAMASTER_SAVE  — saves header row
    ///   2. SP_EVAL_SCHEMASTRUCTURE_SAVE (loop per question) — saves question rows
    /// BOL fields confirmed from DataAccessLayer.dll UTF-16LE analysis.
    /// </summary>
    public class SaveSchemaStructureRequest
    {
        public string SchemaName       { get; set; }
        public string Course           { get; set; }
        public string Regulation       { get; set; }
        public string Sem              { get; set; }
        public string MaxMarks         { get; set; }
        public string MaxNoofQuestions { get; set; }
        public string MaxSections      { get; set; }

        /// <summary>One entry per question row in the schema grid.</summary>
        public List<SchemaQuestionEntry> Questions { get; set; } = new List<SchemaQuestionEntry>();
    }

    /// <summary>
    /// One question row saved via SP_EVAL_SCHEMASTRUCTURE_SAVE.
    /// QStatus: 'O' = Optional, 'C' = Compulsory
    /// </summary>
    public class SchemaQuestionEntry
    {
        public string Qno             { get; set; }
        public string MaxMrk          { get; set; }
        public string QStatus         { get; set; }
        public string MaxNoofQuestions { get; set; }
        public string MaxSections     { get; set; }
    }
}
