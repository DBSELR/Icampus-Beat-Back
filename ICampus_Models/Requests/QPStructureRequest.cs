namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for saving QP (Question Paper) structure.
    /// Maps to SP_EVAL_QPSTRUCTURE_SAVE — saves question paper question selections.
    ///
    /// Page: Evaluation/QP_Structure.aspx
    /// Flow:
    ///   1. Select saved schema → loads compulsory/optional questions into lbQNO1 / lbQNO2
    ///   2. Select questions, click Add → transfers to txtQuestions
    ///   3. Save → SP_EVAL_QPSTRUCTURE_SAVE writes to TBL_EVAL_QPSTRUCTURE
    ///
    /// BOL fields: SchemaName, Questions, QuestionsSubOption, TotalMaxMarks, TotalEnterMaxMarks
    /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    public class SaveQPStructureRequest
    {
        public string SchemaName          { get; set; }
        /// <summary>
        /// Question structure text — pipe/comma separated question codes
        /// e.g. "01A,01B|02A,02B,02C_2" where _2 = best-of-2
        /// Stored in TBL_EVAL_QPSTRUCTURE.QUES column
        /// </summary>
        public string Questions           { get; set; }
        /// <summary>
        /// Sub-option question codes for best-of / section-choice rules
        /// </summary>
        public string QuestionsSubOption  { get; set; }
        public string TotalMaxMarks       { get; set; }
        public string TotalEnterMaxMarks  { get; set; }
    }
}
