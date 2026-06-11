using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for saving an evaluator/scrutinizer registration.
    /// Mirrors Evaluation/Evaluator_Registration.aspx form fields confirmed from ASPX source.
    ///
    /// Flow: SP_EVALUATOR_REGISTRATIONS (save profile)
    ///       → Sp_Eval_Save_UserPapers loop per paper (save paper assignments)
    /// Table: tbl_Eval_Registrations (profile) + tbl_Eval_UserPapers (papers)
    /// </summary>
    public class SaveEvaluatorRequest
    {
        /// <summary>USERID — auto-generated or manual (format: prefix + 3-digit seq)</summary>
        public string UserID      { get; set; }

        /// <summary>NAME — evaluator's full name</summary>
        public string Name        { get; set; }

        /// <summary>DESIGNATION — e.g. "Professor", "Associate Professor"</summary>
        public string Designation { get; set; }

        /// <summary>DEPTARTMENT — branch/department (GRP value from tbl_sh)</summary>
        public string Department  { get; set; }

        /// <summary>USERGROUP — "Evaluator" or "Scrutinizer" (from ddlUserType)</summary>
        public string UserGroup   { get; set; }

        /// <summary>MOBILE — mobile number (max 10 digits)</summary>
        public string Mobile      { get; set; }

        /// <summary>EMAIL — mail address</summary>
        public string Email       { get; set; }

        /// <summary>COLLEGE — college name</summary>
        public string College     { get; set; }

        /// <summary>ISACTIVE — "1" (active) or "0" (inactive) from chkIsActive</summary>
        public string IsActive    { get; set; }

        /// <summary>SEM — semester from ddlsem</summary>
        public string Sem         { get; set; }

        /// <summary>Regulation — from header (needed for Sp_Eval_Save_UserPapers)</summary>
        public string Regulation { get; set; }

        /// <summary>Course — from header (needed for Sp_Eval_Save_UserPapers)</summary>
        public string Course     { get; set; }

        /// <summary>ExamMY — from header (needed for Sp_Eval_Save_UserPapers)</summary>
        public string ExamMY     { get; set; }

        /// <summary>PapCodes — composite paper codes selected in lstPap multi-select listbox</summary>
        public List<string> PapCodes { get; set; } = new List<string>();
    }
}
