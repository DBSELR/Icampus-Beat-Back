using System;

namespace ICampus_DataAccessLayer.Helpers
{
    public enum StoredProcedures
    {
        // ------------------------
        // Login
        // ------------------------
        SP_LOGIN_CHECK,

        // ------------------------
        // CgpService
        // ------------------------
        PROC_LOAD_REGU_BATCH_COURSE,
        PROC_SEARCH_COURSE,
        SP_GROUPMASTER_SAVE,
        PROC_DELETE_COURSE_GRP,
        PROC_COPY_GRP_DATA,

        // ------------------------
        // DropdownService
        // ------------------------
        SPM_COURSE_LIST,
        SPM_EXAMS_ExamMY_Load,
        SPM_Course_ExamMY_to_User,
        SPM_BATCH_LIST,
        SPM_BRANCH_LIST,
        SPM_SEM_LIST,
        SPM_STREAM_LIST,
        SPM_PCODE_LIST,
        proc_getpap,
        sp_PAP_Save,
        SPM_GPAP_PAP_DEL,
        sp_GPAP_SAVE,
        sp_SH_Change_Pap,
        sp_GRP_NewBatch,
        sp_RegsUp_Check,

        // ------------------------
        // StudentService (from DAL)
        // ------------------------

        sp_stdDATA_Save,
        sp_stdDATA_Load,
        sp_stdDATA_Load_Details,
        sp_SH_Inactive,
        sp_SH_Reactive,
        SP_READMISSION,
        PROC_DETAINEDLIST,
        PROC_PASSED_ALL_SEMS,
        PROC_BranchWiseCount,
        PROC_Readmissionlist,
        SPR_STDDATA_FOR_ICAMPUS,
        SPL_EMPREGISTRATION_SAVE,
        SP_Facultydata,

        // For Exams
        SP_EXAMS_SAVE_API,
        PROC_EXAM_MASTERLIST,
        SPM_ExamNofications_Save,
        SPM_RESETREGSUP_EXAMS,
        sp_RegsUp_Save,
        SPM_EXISTING_EXAMS_LOAD,
        SPM_EXAMNOFICATIONS_LIST,
        PROC_EXAMMASTER_DELETE,
        SP_BATCH_RESET,
        SPM_GETBATCH_COURSE,

        // ------------------------
        // FeeService
        // ------------------------
        PROC_LOAD_SEMS_FOR_FEE,
        SPM_FEESTUCTURE_LOAD,
        SP_FEESTRUCTURE_SAVE,
        PROC_SUPPLY_FEE_GRIDLOAD,
        PROC_SUPPLY_FEE_SAVE,
        SP_FAILD_SUBJECTS_COUNT,
        PROC_FINE_FEE_SAVE,
        SP_LOAD_MAX_SEM,
        SP_CONDINATION_SEMS,
        SP_CONDINATION_DATES_LOAD,
        SP_CONDINATION_DATES_SAVE,
        SP_CONDINATION_DATES_DELETE,

        // ------------------------
        // MasterService (new)
        // ------------------------

        /// <summary>
        /// Checks PAP and master creation status
        /// </summary>
        SPM_PAP_CHECK_MASTERCREATION,

        /// <summary>
        /// Updates paper (subject) information
        /// </summary>
        PROC_UPDATE_PAP,

        /// <summary>
        /// Loads regular master data
        /// </summary>
        SPM_RegularMasterData_LOAD,

        /// <summary>
        /// Checks if master already exists for given course/exam/regu/sem
        /// </summary>
        SPM_MASTER_EXISTS_CHECK,

        /// <summary>
        /// Creates regular master data
        /// </summary>
        SP_MASTER_CREATE,

                // Subject/Grade related
        PROC_BATCH_LOAD,                    // PROC_BATCH_LOAD  @course
        PEOC_LAOD_GRADEMASTER_GRIDS,        // PEOC_LAOD_GRADEMASTER_GRIDS @TYPE, @course, @REGU
        SP_GRADE_SAVE,                      // SP_GRADE_SAVE @ID,@REGU,@MRKFROM,@MRKTO,@GR,@GRPTS,@course
        PROC_COPY_GRADE_DATA,               // PROC_COPY_GRADE_DATA @REGU,@PREGU,@COURSE,@TYPE
                                            // (If you need sem-grade SP names also:)
        sp_LOAD_GRADEMASTER_GRIDS,           // sp_LOAD_GRADEMASTER_GRIDS @TYPE,@course,@REGU (optional)


        SP_LOAD_GRADEMASTER_GRIDS,   // your file uses sp_load_GRADEMASTER_GRIDS
        SP_SEMGRADE_SAVE,


        SPM_ROOMMASTER_SAVE,
        SPM_RoomMaster_List,
        SPM_CheckRoomPriority,
        SPM_BranchPriority_List,
        SPM_BRANCHPRIORITY_SAVE,

        // Example placement in StoredProcedures enum
        SPM_ELECTIVEDATA_IMPORTING,
        SP_Import_Exam_Session_Date,
        SP_IMPORT_CONDONATION_FEE,
        Proc_Import_Registration_block,
        Proc_Import_Halltickets_block,
        Proc_Import_Result_block,
        Proc_Import_Internal_ImproveMarks,
        Proc_Import_AuditCourse_Data,
        Proc_Import_RCRV_Marks,
        SP_EXCEL_FORMAT_ExtraColumns,
        sp_stdDATA_Importing,

        PROCFEEHEADS_SAVE,

       // PROC_BATCH_LOAD,
        sp_LOAD_CLASSMASTER_GRIDS,
        SP_CLASSGRADE_SAVE,
        PROC_COPY_GRADE_Class_DATA,

        SP_MASTER_CREATE_REGNO,
        PROC_GETSUBJTS_REGNO_WISE,
        PROC_OMRNUM_UPDATE,
        PROC_OMRNUM_UPDATE_Get,
        PROC_UPDATE_OMRNO,
        SP_OMRSAVEMARKSENTRY,
        sp_loadstdexammy_update,
        sp_Exammy_update,
        PROC_marksupdate_regnowise,
        PROC_DEL_ASHID,



        //    SP_BRANCHMASTER_SAVE,
        //Sp_RoomAllot_Load_Course,// INSERT/UPDATE branch master (room/branch priority)
        //SP_CHECK_ROOM_PRIORITY,         // check priority exists (returns INT)
        //SP_LOAD_BRANCH_PRIORITY,        // load branch priority rows for given id/session
        //SP_UPDATE_BRANCH_PRIORITY,      // update priority order (accepts @UP_Q)
        //SP_DELETE_BRANCH_PRIORITY,       // delete a branch entry (by priority/SEM/Branch/EDate/Session)


        SPM_SUBJECTLIST,
        PROC_LOAD_PAPERSLIST,

        PROC_IS_IASUPDATE,          // used by IAS_Report()
        SP_REGNO_EXAMMY,            // maps to sp_regno_exammy
        PROC_RESULTPROCESS_ALLDATA,  // maps to proc_resultprocess_alldata

        PROC_RESULTPROCESS_ALLDATAA, // proc_resultprocess_alldataa (if you use the alternate name)

        SP_BRANCH_MASTER_SAVE,
        SP_CHECK_ROOM_PRIORITY,
        SP_LOAD_BRANCH_PRIORITY,
        SP_UPDATE_BRANCH_PRIORITY,
        SP_DELETE_BRANCH_PRIORITY,

        // Room / Branch related
        Sp_RoomAllot_Load_Course,

        SEMS_FOR_EXAMREG,
        SPM_STUDENT_DETAILS,
        SPM_REQ_DATA_FOR_EXAMREG,
        sp_RegsUp_Check_ExammY,
        SPM_GET_FEESTRUCTURE_REG,
        SPM_GET_FEESTRUCTURE_SUP,
        SP_SH_EXAMREG_SAVE,
        sp_SH_ExamReg_Update,
        SPM_EXAMFEE_PAY,
        SPM_EXAMFEE_RECEIPT,
        SPM_EXAM_REGISTER_LIST,
        SPM_ExamReg_Pap,
        sp_SH_ExamReg_Pap,
        SPM_Get_ExamReg_Pap_Temp,
        Dup_Receipt,
        SPM_ExamUnRegistration,
        SPM_EXAMNOTIFICATION_CHECK,
        sp_SH_ExamReg_Check,
        SPM_EXAMFEE_RECEIPT_CHECK,

        SPS_Get_Sems_ExamMY,
        SPS_GetPapersWithCode,
        SPS_Get_Papers_Data,
        SPS_Get_ExamTimeTableData,
        SPS_Update_ExamSession,
        SPS_Update_ExamDate,
        SPS_Get_ExamDates,
        SPS_Get_RAPapers_Data,
        SPS_Update_RoomNumbers,
        SP_EXAMSESSIONDATEFORMAT,

        SPM_LOAD_SEMS_FOR_RV,
        SPM_GETPAPERS_FOR_RV,
        SP_RV_FEE_DATA,
        SPM_GET_OPT_RV_PAPERS,
        SPM_GET_RV_BUNDLE_SCRIPTS,
        SP_RV_CLOSINGDATES,

        PROC_MISC_FEEPAY,
        Sp_LoadFee_Data,
        PROC_LOADRECEIPT,    // your DAL called PROC_LOADRECEIPT
        SP_Export_MiscFee,

        PROC_DUP_CERTIFICATE_DATA,
        SPM_HT_LBRCE,
        SP_MRK_MEMO,

        SP_GETDETAILS,
        SP_EXAMFEECONCESSION_SAVE,
        SP_FeeConcession_Grid,
        SPM_GET_FEEConcession_REG,
        SPM_GET_FEEConcession_SUP,

        Sp_RoomAllot_Load_Regno,
        Sp_Std_Exam_RoomAllotment,
        Sp_RoomAllotment_Load_Data,
        Sp_Reset_Room,
        Sp_room_ExcelExport_Dump,
        SPS_GET_EDATE,
        SPR_LOAD_ROOM,
        proc_Get_Elec_Subjects,
        Spr_Load_Session,
        proc_room_allotment_For_AllDates,
        Sp_Get_Std_Data,

        SP_CONDONATION_LOAD_SEMS,
        SP_CONDONATION_GET_STUDENT,
        SP_Condotion_Grid,
        SP_Condinatiom_date_CHECK,
        SP_CONDONATION_SAVE,
        SP_CONDONATION_FORMAT,
        PROC_CONDONATION_EXPORT,
        SP_GetDetails,

        // add these to ICampus_DataAccessLayer.Helpers.StoredProcedures
        SPM_EXAMFEE_COLLECTION,
        Sp_Receipt_List,
        sp_Search_Regno,


            //SPM_STUDENT_DETAILS,
        PROC_FEERECEIPT_SUBJECTS,
        SPM_RECEIPT_CANCELED,

        // ------------------------
        // Fee Reports
        // ------------------------
        PROC_FEEE_LIST_OVERALL,

        // ------------------------
        // Supply Lab Registered
        // ------------------------
        SP_SUPPLYLABREGISTEREDDATA,

        // ------------------------
        // Credits Mismatch
        // ------------------------
        SPM_CREDITSMISMATCH,

        // ------------------------
        // Exam UnRegistration
        // ------------------------
        PROC_GET_DATA,
        PROC_REGDUPDATE,

        // ------------------------
        // Unblock Registrations
        // ------------------------
        PROC_GET_UNBLOCK,
        PRO_SAVE_UNBLOCK,

        // ------------------------
        // TimeTable
        // ------------------------
        SPM_TIMETABLE_LOAD,

        // ------------------------
        // Question Paper Statement
        // ------------------------
        SPM_QPSTATEMENT,

        // ------------------------
        // OMR Sheet
        // ------------------------
        SP_REP_OMRSHEET,
        sp_SH_Omrnumber,
        SP_OMRDATA_EXPORT,

        // ------------------------
        // Nominal Rolls
        // ------------------------
        SP_REP_NOMINALROLLS,
        SP_REP_NOMINALROLLS_Readmit,

        // ------------------------
        // Cancel Receipt List
        // ------------------------
        SP_Cancel_Receipt,

        // ------------------------
        // Seating Arrangement
        // ------------------------
        Sp_temproom_Dump,
        Proc_Load_Edate,

        // ------------------------
        // Room Abstract
        // ------------------------
        SPR_LOAD_EXAMDATES,
        SPR_LOAD_EXAMDATES_Supple,

        // ------------------------
        // Mid Hall Tickets
        // ------------------------
        SPM_HallTicket_Mid,

        // ------------------------
        // RoomWise Nominal Rolls
        // ------------------------
        Sp_REP_NominalRolls_ROOMWISE,
        Sp_REP_Nominal_LoadEdate,
        Sp_REP_Nominal_LoadBranch,

        // ------------------------
        // Internal Marks Entry (Post-Exams)
        // PROC_LOADPAPERS_MRKENTRY  : loads papers dropdown (TYPE='I' for internal)
        // PROC_LOADMARKS_MRKENTRY   : loads student marks grid (TYPE='I' for internal)
        // PROC_UPDATE_MARKS_INT_S_T : saves single student mark (TYPE='S' for SMARKS)
        // ------------------------
        PROC_LOADPAPERS_MRKENTRY,
        PROC_LOADMARKS_MRKENTRY,
        PROC_UPDATE_MARKS_INT_S_T,

        // ------------------------
        // Mid Absentees Entry (Post-Exams)
        // PROC_LOADMARKS_MRKENTRY_MID  : loads student marks grid for Mid exams (TYPE='T', ExamType)
        // PROC_UPDATE_MID_MARKS_INT_S_T: saves single student AB/MP code for Mid (TYPE='T', ExamType)
        // Note: papers dropdown reuses PROC_LOADPAPERS_MRKENTRY (same 6 params, TYPE='T')
        // ------------------------
        PROC_LOADMARKS_MRKENTRY_MID,
        PROC_UPDATE_MID_MARKS_INT_S_T,

        // ------------------------
        // Post Exam-Reports: Internal Check List
        // SP_REP_INTERNAL_CHKLIST: returns internal marks checklist rows
        //   params (5): Course, ExamMY, Regulation, GRP (optional), SEM (optional)
        // Confirmed from DLL IL: App_Web_oxqewfcs.dll loadingInternalCheckList method
        // ------------------------
        SP_REP_INTERNAL_CHKLIST,

        // ------------------------
        // Post Exam-Reports: Practical Check List
        // SP_REP_PRAC_CHKLIST: returns practical marks checklist rows
        //   params (6): Course, ExamMY, Regulation, GRP (optional), SEM (optional), PCODE (always empty - ddlPcode hidden in UI)
        // Confirmed from DLL IL: App_Web_gp3pforx.dll loadingInternalCheckList method
        // ------------------------
        SP_REP_PRAC_CHKLIST,

        // ------------------------
        // Post Exam-Reports: D Form (DFORM.aspx)
        // sp_rep_deform        : regular D Form report
        // sp_rep_deform_Readmit: readmission D Form report
        // params (5 each, all varchar): Regulation, Course, ExamMY, SEM (optional), EDate (optional)
        // Confirmed from DLL IL: App_Web_gp3pforx.dll loadingdform method (IL offset 0x0000c3be)
        // EDate is stored as yyyy-MM-dd format
        // ------------------------
        sp_rep_deform,
        sp_rep_deform_Readmit,

        // ------------------------
        // Post Exam-Reports: Absentees List (Theory)
        // SP_REP_ABLIST: returns theory absentees list
        //   params (5): Regulation(varchar), Course(varchar), ExamMY(varchar), SEM(INT unquoted optional), EDate(varchar yyyy-MM-dd optional)
        // Confirmed from DLL IL: App_Web_gp3pforx.dll iCampus_Reports_AbsenteesList btnView_Click (offset 0x000128e7)
        // Note: LabAbsenteesList uses inline SQL (no SP) — PMARKS IN ('ab','sm') filter on tbl_SH
        // ------------------------
        SP_REP_ABLIST,

        // ------------------------
        // Post Exam-Reports: Subject Wise Present List (Studentwise_presentlist.aspx)
        // SP_Pcode_Presentlist: returns pcode present list for export
        //   params (5, all varchar): Course, Regulation, ExamMY, Sem, Regsup
        // Confirmed from DLL IL: DataAccessLayer.dll DAL_StudentHistory::Pcode_PresentList (US heap US[0xc0c1])
        //   Exec template: "SP_Pcode_Presentlist 'Course','Regulation','ExamMY', 'Sem', 'Regsup'"
        //   All 5 parameters are quoted = all varchar
        // Semesters dropdown: inline SQL on TBL_SH — 3 params: ExamMY, Course, Regulation
        // ------------------------
        SP_Pcode_Presentlist,

        // ------------------------
        // Post Exam-Reports: D Form Mid (DFORM_MID.aspx)
        // sp_rep_deform_MID        : regular MID D Form report
        // sp_rep_deform_Readmit_MID: readmission MID D Form report
        // params (6): Regulation(varchar), Course(varchar), ExamMY(varchar),
        //             SEM(INT — unquoted!), EDate(varchar, yyyy-MM-dd), ExamType(varchar "1"=MID-I "2"=MID-II)
        // Confirmed from DLL IL: App_Web_gp3pforx.dll iCampus_Reports_DFORM_MID (ldstr offset 0xc4f1/0xc3be)
        //   Phase analysis: SEM is unquoted (int), EDate is DateTime.Parse+ToString("yyyy-MM-dd"), ExamType is quoted varchar
        //
        // proc_LOAD_MID_DATES: loads exam dates for ddlEdate cascade (ddlSemester_SelectedIndexChanged)
        // params (5, all varchar): Regulation, Course, ExamMY, Sem, ExamType
        // Returns column: MDATE
        // Confirmed from DLL IL: DataAccessLayer.dll DForm_Edate_MID (US heap 0x9d61, file 0x26bd5)
        //   Exec template: "proc_LOAD_MID_DATES 'Regulation','Course','ExamMy','Sem','ExamType'"
        // Semesters dropdown: inline SQL on TBL_SH — 3 params: Course, ExamMY, Regulation
        //   SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regulation
        // ------------------------
        sp_rep_deform_MID,
        sp_rep_deform_Readmit_MID,
        proc_LOAD_MID_DATES,

        // ------------------------
        // UserSettings (Settings / User Form(s)) — App_Web_21gpaxix.dll UserSettings.aspx.cs
        // SPM_FORMS: returns all available menu/sub-menu forms (no params)
        //   Confirmed: PageMethod getForms() calls SPM_FORMS
        // SPM_UserGroup_Menu_Load: returns forms assigned to a user group
        //   Confirmed: PageMethod checkusergroup(group) — 1 param: userGroup (varchar)
        // SPM_USERS_MENU_DELETE: deletes all forms for a user group
        //   Confirmed: PageMethod DeleteUserForms(group) — 1 param: userGroup (varchar)
        // SPM_USERS_MENU_SAVE: saves one form entry for a user group
        //   Confirmed: PageMethod SaveUserForms(group,cid,subid) — 3 params: userGroup, menuId, subMenuId
        // PROC_DEL_USERS: deletes an entire user group
        //   Confirmed: PageMethod DeleteUserGroup(group) — 1 param: userGroup (varchar)
        // TBL_USERS_MENU inline SQL: SELECT DISTINCT USERID FROM TBL_USERS_MENU
        //   Confirmed: PageMethod UserGroups_Settings() — no params
        // ------------------------
        SPM_FORMS,
        SPM_UserGroup_Menu_Load,
        SPM_USERS_MENU_DELETE,
        SPM_USERS_MENU_SAVE,
        PROC_DEL_USERS,

        // ------------------------
        // Evaluation — Schema Structure (Evaluation/Schema_Structure.aspx)
        // SP_EVAL_SCHEMAMASTER_SAVE  : saves schema master header row
        //   params: @SchemaName, @MaxMarks, @MaxNoofQuestions, @MaxSections, @Course, @Regulation, @Sem
        // SP_EVAL_SCHEMASTRUCTURE_SAVE : saves one question row for a schema (called in loop)
        //   params: @SchemaName, @Qno, @MaxMrk, @QStatus, @MaxNoofQuestions, @MaxSections
        // SP_EVAL_LOAD_STRUCTURE_Edit  : loads schema master + structure rows for edit form
        //   params: @SchemaName
        // SP_EVAL_LOAD_STRUCTURE       : loads schema structure for display (view mode)
        //   params: @SchemaName
        // Sp_Eval_Check_Schema         : checks if schema name already exists (returns count)
        //   params: @SchemaName
        // Sp_Eval_Delete_Pap_Data      : deletes a schema and all its question rows
        //   params: @SchemaName
        // Confirmed from DataAccessLayer.dll UTF-16LE string analysis (BOL_Eval_Structure fields)
        // ------------------------
        SP_EVAL_SCHEMAMASTER_SAVE,
        SP_EVAL_SCHEMASTRUCTURE_SAVE,
        SP_EVAL_LOAD_STRUCTURE_Edit,
        SP_EVAL_LOAD_STRUCTURE,
        Sp_Eval_Check_Schema,
        Sp_Eval_Delete_Pap_Data,

        // ------------------------
        // Evaluation — Apply Schema (Evaluation/Apply_Schema.aspx)
        // Sp_Eval_Load_Sem        : loads semester dropdown for apply-schema form
        //   params: @Course, @Regulation, @ExamMY (all varchar)
        // Sp_Eval_Load_Papers     : loads available papers grid (pcode, Pname, SStatus)
        //   params: @Course, @Regulation, @Sem, @ExamMY (all varchar)
        // Sp_Eval_Get_PapData     : gets papers already assigned to a schema
        //   params: @SchemaName, @Course, @Regulation, @Sem (all varchar)
        //   Returns: Id, PapCode+'_'+PNAME, NoofQuestions, MaxSections
        //   Confirmed inline SQL: Select distinct Id,PapCode+'_'+PNAME as PapCode,...
        //     from TBL_EVAL_SCHEMAMASTER S inner join tbl_pap P on ...
        // Sp_Eval_Save_UserPapers : applies a schema to one paper (called per selected paper)
        //   params: @SchemaName, @PapCode, @Course, @Regulation, @Sem, @ExamMY (all varchar)
        // Note: Sp_Eval_Delete_Pap_Data also used here with @SchemaName, @PapCode
        //   to delete one paper-schema row (same SP, extra @PapCode param)
        // Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll UTF-16LE analysis
        // ------------------------
        Sp_Eval_Load_Sem,
        Sp_Eval_Load_Papers,
        Sp_Eval_Get_PapData,
        Sp_Eval_Save_UserPapers,
        Sp_Eval_Remove_UserPaper,

        // ------------------------
        // Evaluation — Evaluator Registration (Evaluation/Evaluator_Registration.aspx)
        // SP_EVALUATOR_LOAD           : loads evaluator/scrutinizer list grid
        //   params: @UserGroup (varchar — "Evaluator" or "Scrutinizer")
        //   Returns: EID, USERID, NAME, DESIGNATION, DEPTARTMENT, USERGROUP, MOBILE, EMAIL, COLLEGE, ISACTIVE
        // SP_EVALUATOR_LOAD_USER_DETAILS : loads full details of one evaluator for edit
        //   params: @UserID (varchar)
        // SP_EVALUATOR_REGISTRATIONS  : saves evaluator/scrutinizer details
        //   params: @UserID, @Name, @Designation, @Department, @UserGroup, @Mobile, @Email, @College, @IsActive, @Sem
        // SP_OE_EVALUATOR_LIST        : returns active evaluator dropdown
        //   (no params / used in Scripts Assign page too)
        //   Returns: USERID, NAME for evaluator selection
        //
        // Inline SQL confirmed from DataAccessLayer.dll:
        //   Auto-generate UserID: select isnull(max(right(UserID,3)),0)+1 as UserID
        //     from tbl_Eval_Registrations where USERGROUP='...'
        //   Load user papers: Select PCode,PCode+'_'+PNAME as PNAME
        //     from tbl_Eval_UserPapers where UserId='...'
        //   Load dept list: select distinct GRP from tbl_sh
        //     where Regulation='...' and Course='...' and EXAMMY='...' order by GRP
        //
        // Note: Sp_Eval_Save_UserPapers also used here per paper (reused enum from Apply Schema)
        //   In this context: params @UserId, @PapCode
        // Confirmed from App_Web_xplim0cm.dll ASPX source analysis
        // ------------------------
        SP_EVALUATOR_LOAD,
        SP_EVALUATOR_LOAD_USER_DETAILS,
        SP_EVALUATOR_REGISTRATIONS,
        SP_OE_EVALUATOR_LIST,

        // ------------------------
        // Evaluation — Scripts Assign (Evaluation/Scripts_Assign.aspx)
        // Sp_Eval_Script_Assign_Load_Sem : loads Sem dropdown after subject selected
        //   params: @EvaluatorId, @PapCode (both varchar)
        // SP_Eval_Get_BundleNo           : loads bundle number list for lstBundleNo
        //   params: @PapCode, @Sem (both varchar)
        //   Source table: Tbl_DV_Marks
        // SP_Eval_Get_Scripts            : loads scripts (answer booklet IDs) in a bundle
        //   params: @PapCode, @Sem, @BundleNo (all varchar)
        //   Filter: EvaluatorId IS NULL (unassigned only)
        // SP_EVAL_SAVE_SCRIPTS           : assigns scripts to evaluator + stores file paths
        //   params: @EvaluatorId, @PapCode, @Sem, @EvalDate, @BundleNo, @ScriptIds, @QpPath, @KeyPath
        //
        // Inline SQL confirmed from DataAccessLayer.dll:
        //   Pending count: select COUNT(*) as PendingScripts from Tbl_DV_Marks where EvaluatorId is null
        //   Evaluator subjects: Select PCode,PCode+'_'+PNAME as PNAME from tbl_Eval_UserPapers
        //     where UserId='...' order by PName
        //
        // File uploads: QP → Evaluation/QuestionPaper/, Key → Evaluation/Key/
        // Success message: "Scripts assign to Evaluator successfully.."
        // Confirmed from App_Web_xplim0cm.dll ASPX source + DataAccessLayer.dll analysis
        // ------------------------
        Sp_Eval_Script_Assign_Load_Sem,
        SP_Eval_Get_BundleNo,
        SP_Eval_Get_Scripts,
        SP_EVAL_SAVE_SCRIPTS,
        SP_EVAL_LOAD_STRUCTURE_MarksEntry,
        Sp_Eval_Get_Qp_Structure,
        SP_EVAL_QPSTRUCTURE_SAVE,
        Sp_Eval_Get_SecuredMarks,
        Sp_Eval_Insert_Student_SecuredMarks,

        // ------------------------
        // Results — Pending List (Results/PendingList.aspx)
        // SPS_Get_InternalPendingList  : returns internal marks pending list
        // SPS_Get_PracticalPendingList : returns practical marks pending list
        // SPS_Get_TheoryPendingList    : returns theory marks pending list
        // SPS_Get_RVPendingList        : returns revaluation marks pending list
        //   params (all varchar): @EXAMMY, @COURSE, @Regulation
        // Confirmed from: App_Web_m2jhophz.dll (iCampus_Results_PendingList)
        //   BAL_PendingList -> DAL_PendingList -> BOL_PendingList
        //   BOL properties: Course, ExamMY, Regulations (from master page session)
        //   Radio buttons: RBInternalPendingList, RBPracticalPending,
        //                  RBTheoryPending, RBRVPending
        // ------------------------
        SPS_Get_InternalPendingList,
        SPS_Get_PracticalPendingList,
        SPS_Get_TheoryPendingList,
        SPS_Get_RVPendingList,

        // ------------------------
        // Results — Grofting / Flotation (Results/Grofting.aspx)
        // SP_GRACING_GROFTING : runs the grafting/grofting process for a paper-sem
        //   params (all varchar): @Course, @ExamMY, @Semester, @PaperCode
        //   Note: old project calls it "Grofting Process" / "Grafting Process"
        //   After running this, Result Process must be run
        // Confirmed from: App_Web_m2jhophz.dll (iCampus_Results_Grofting),
        //   DataAccessLayer.dll UTF-16LE US heap string "SP_GRACING_GROFTING '"
        // ------------------------
        SP_GRACING_GROFTING,

        // ------------------------
        // Results — Result Process (Results/RegnoWiseResultProcess.aspx)
        // SP_RESULT_PROCESS_REGNOWISE : runs result process for a single student (regno-wise)
        //   params: @Regulation, @COURSE, @EXAMMY1, @SEM, @GRP, @REGNO, @flag('NORMAL'|'SM'|'GR'|'RV')
        //   Confirmed: EXEC SP_RESULT_PROCESS_REGNOWISE 'R20','B.Tech','May-2024','8','CE','20671A0101','NORMAL'
        // SP_RESULT_PROCESS         : batch result process (not regno-wise)
        //   params: @Regulation, @COUSRE(typo), @EXAMMY1, @SEM, @flag, @GRP=''
        // SP_RESULT_PROCESS_readmit : readmit result process
        //   params: @Regulation, @COURSE, @EXAMMY1, @SEM, @flag, @GRP, @READMIT_REGULATION
        // PROC_EXAMMY_CHK_RES_PROCESS : checks ExamMY validity (5 params: @COURSE,@EXAMMY,@SEM int,@RES_DATE,@PROC_TYPE)
        //   Not used in API — too complex, dropdown already validates via SPM_EXAMS_ExamMY_Load
        // PROC_RESULT_LOAD_REGU_COURSE_GRP : loads batch/regu dropdown for result process
        //   params (varchar): @Course
        // SP_SEMS_RP : loads sems for result process
        //   params (varchar): @Course, @ExamMY
        // Confirmed from: App_Web_m2jhophz.dll (iCampus_Results_RegnoWiseResultProcess),
        //   DataAccessLayer.dll UTF-16LE US heap strings
        // ------------------------
        SP_RESULT_PROCESS,
        SP_RESULT_PROCESS_readmit,
        SP_RESULT_PROCESS_REGNOWISE,
        PROC_EXAMMY_CHK_RES_PROCESS,
        PROC_RESULT_LOAD_REGU_COURSE_GRP,
        SP_SEMS_RP,

        // ------------------------
        // Results — Moderation (StudentResult.aspx moderation tab)
        // PROC_MODERATION_REG_SEM_GRP_PAP : loads moderation marks grid
        //   params (all varchar): @Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode
        //   Name decodes: REG=Regu/batch, SEM, GRP=branch, PAP=paper
        // PROC_MODERATION_AND_MCNT        : saves moderation marks + returns count
        //   params (all varchar): @Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode, @ModMarks
        // proc_moderation_new             : new version of moderation SP (brackets = old project name)
        //   params (all varchar): @Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode, @ModMarks
        // Confirmed from: DataAccessLayer.dll UTF-16LE US heap strings
        //   "PROC_MODERATION_REG_SEM_GRP_PAP '" and "PROC_MODERATION_AND_MCNT '"
        //   BAL method names: Update_Moderation, PreModeration, Check_Moderation_Cnt
        // ------------------------
        PROC_MODERATION_REG_SEM_GRP_PAP,
        PROC_MODERATION_AND_MCNT,
        proc_moderation_new,

        // ------------------------
        // Results — Student History (Results/StudentHistory.aspx)
        // SPM_STUDENTHISTORY        : loads the full subject-wise history grid for a student
        //   params: @RegNo (varchar)
        //   Returns: ASHID, REGNO, PCODE, PNAME, CR, SMARKS, MRK_FIN, MARKS, SEM, GR, GRPTS, EXAMMY
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap "SPM_STUDENTHISTORY '"
        //   BAL method: dgvStudentHistory data bind in displaystddata
        // SPM_STUDENT_DETAILS       : loads student personal info (already in enum at line 194)
        //   params: @RegNo (varchar)
        //   Returns: Name, Programme/Course, Branch/GRP etc.
        // PROC_SGPA_AVERAGE         : loads SGPA/CGPA per semester for the SGPA/CGPA grid
        //   params: @RegNo (varchar)
        //   Returns: SEM, SGPA, CGPA, TCR (Total Credits), SecuredCR, BackLogs
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap "PROC_SGPA_AVERAGE '"
        //   BAL method: get_rgnowise_SgpaCgpa (gvSGPA_CGPA data bind)
        // PROC_DEL_ASHID            : deletes a TBL_SH record by ASHID (already in enum at line 163)
        //   params: @ASHID (varchar or int)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap "PROC_DEL_ASHID '"
        //   BAL method: Delete_ashid
        // SPM_Student_MaxExamMY     : gets the maximum (latest) ExamMY for a student
        //   params: @RegNo (varchar)
        //   Returns: MaxExamMY (latest exam period — used to auto-trigger result process)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap "SPM_Student_MaxExamMY '"
        //   BAL method: getStudentMaxExamMY (called after SetStudentMarks)
        // Raw SQL — get marks by ASHID (for edit modal load):
        //   SELECT PCODE, PNAME, TMARKS, MMARKS, RVMARKS, V3, MRK_FIN, SMARKS, PMARKS
        //     FROM TBL_SH WHERE ASHID = @ASHID
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 83386
        // Raw SQL — update marks by ASHID (SetStudentMarks, btnSave_Click):
        //   UPDATE TBL_SH SET PNAME=@PName, SMARKS=@SMarks, TMARKS=@TMarks,
        //     MMARKS=@MMarks, RVMARKS=@RVMarks,
        //     V3 = CASE WHEN IS_V3='Y' THEN @V3 ELSE NULL END, PMARKS=@PMarks
        //   WHERE ASHID = @ASHID
        //   Confirmed: DataAccessLayer.dll UTF-16LE fragment (offset 83437–83531)
        // ------------------------
        SPM_STUDENTHISTORY,
        PROC_SGPA_AVERAGE,
        SPM_Student_MaxExamMY,

        // ------------------------
        // Results — ReAdmission (Results/ReAdmission.aspx)
        // SPM_ReAdmissionsStudentMarks  : loads marks for one TBL_SH record into edit modal
        //   params: @ASHID (varchar — set by clicking a PNAME LinkButton in the grid)
        //   Returns: PCODE, PNAME, TMARKS, MMARKS, RVMARKS, SMARKS, PMARKS + max/pass fields
        //   Confirmed: DataAccessLayer.dll UTF-16LE "SPM_ReAdmissionsStudentMarks ?"
        //   BAL method: getReAdmissionStudentMarks (triggered by set_aSHID → modal show)
        // SPM_PaperDetails_ReAdmission  : loads paper structure when paper code is typed
        //   params: @PCode (varchar — txtPCode AutoPostBack TextChanged)
        //   Returns: PNAME, PTYPE, CR, SGPA_CR, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS, P1, P2, ASG
        //   Confirmed: DataAccessLayer.dll UTF-16LE "SPM_PaperDetails_ReAdmission ?"
        //   BAL method: getPaperDetails (txtPCode_TextChanged → fills modal max/pass/type)
        // Raw SQL — SELECT DISTINCT PTYPE (loadPaperType / cmbEntryType):
        //   SELECT DISTINCT Cast(PTYPE as varchar(50)) PTYPE FROM TBL_PAP
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 75957 (no params)
        // Raw SQL — UPDATE marks for readmit (SetReAdmissionsStudentMarks / btnSave_Click):
        //   UPDATE TBL_SH SET PCODE=@PCode, PNAME=@PName, REGU=@Regulation, SEM=@Sem,
        //     TMARKS=@TMarks, MMARKS=@MMarks, RVMARKS=@RVMarks, SMARKS=@SMarks, PMARKS=@PMarks,
        //     PTYPE=@EntryType, CR=@Credits, SGPA_CR=@SgpaCr,
        //     TMAX=@TMax, TPASS=@TPass, SMAX=@SMax, SPASS=@SPass,
        //     PMAX=@PMax, PPASS=@PPass, MAXMRK=@MaxMrk, PASS=@Pass,
        //     elec = CASE WHEN (PCODE != tempcode AND elec IS NULL) THEN 'R' ELSE elec END
        //   WHERE ASHID = @ASHID
        //   Confirmed: DataAccessLayer.dll UTF-16LE fragment "elec =case when (pcode!= tempcode
        //     and elec is null) then 'R' else elec end ? WHERE ASHID = ?" (offset 75884)
        //   BOL props used: Regulations, Credits, SGPA_Credits, PMax/SMax/TMax, PPass/SPass/TPass/Pass, etc.
        // PROC_DEL_ASHID (reused from StudentHistory — already in enum)
        // SPM_STUDENT_DETAILS (reused — already in enum)
        // SPM_STUDENTHISTORY (reused — already in enum above)
        // ------------------------
        SPM_ReAdmissionsStudentMarks,
        SPM_PaperDetails_ReAdmission,

        // ------------------------
        // Results — BackLogs List (Results/BackLogsList.aspx)
        // SP_BACKLOGS_LIST  : main backlogs list filtered by course, batch, exammy, sem range, op, no. of backlogs
        //   params (all varchar): @Course, @REGU, @ExamMY, @SemFrom, @SemTo, @NoOfBackLogs, @Op
        //   Returns: REGNO, SNAME, SECTION, GRP, SEM, PCODE, PNAME, LAST ATTEMPT
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 119682 "SP_BACKLOGS_LIST "
        //   BAL method: BackLogsList (btnBackLogsList_Click)
        //   SQL fragment: SEM BETWEEN @SemFrom AND @SemTo,
        //                 CONVERT(DATE,'01-'+EXAMMY,105) <= CONVERT(DATE,'01-'+@ExamMY,105),
        //                 REGU = @REGU, and no. of backlogs @Op @NoOfBackLogs
        //   Operator stored in state: btnEquation cycles =, <=, >=
        //
        // Results — Backlogs_Regno (Results/Backlogs_Regno.aspx)
        // PROC_Backlogs_RegNowise : regno-wise detailed backlogs list (BackLogsData button)
        //   params (all varchar): @Course, @ExamMY, @REGU
        //   Returns: REGNO, SNAME, SEM, PCODE, PNAME, and related backlog fields
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 120392 "[PROC_Backlogs_RegNowise]"
        //   BAL method: Backlogs_Regno (btnBacklosData_Click)
        //
        // Sp_BacklogsData  : backlogs count per student (BackLogsCount button)
        //   params (all varchar): @Course, @ExamMY, @REGU
        //   Returns: REGNO, SNAME, and count/summary fields
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 120446 "[Sp_BacklogsData]"
        //   BAL method: Backlogs_Count (btnBackLogsCount_Click)
        //
        // Batch dropdown (both pages) — raw SQL:
        //   SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH
        //   FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU
        //   Confirmed: DataAccessLayer.dll UTF-16LE fragment before SP_BACKLOGS_LIST
        // ------------------------
        SP_BACKLOGS_LIST,
        PROC_Backlogs_RegNowise,
        Sp_BacklogsData,

        // ------------------------
        // Results — Toppers List (Results/ToppersList.aspx)
        // PROC_TOPPERSLIST_NEW   : main toppers list (btnToppersList_Click when chksemwise=false)
        //   params (all varchar): @Course, @REGU, @Sem, @NoOfToppers, @Branch, @Caste, @Gender, @WithRv
        //   Returns: REGNO, SNAME, COURSE, BRANCH, CASTE, SEM, GENDER,
        //            TOTAL GRADE POINTS, TOTAL CREDITS, SGPA, CGPA, RANK, exammy, regsup
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 169649 "[PROC_TOPPERSLIST_NEW]"
        //   BAL method: Get_Toppers_List (btnToppersList_Click)
        //   Grouping flags: Branch=1 groups by branch, Caste=1 groups by caste, Gender=1 by gender
        //
        // PROC_TOPPERSLIST_SemWise : semester-wise toppers (btnToppersList_Click when chksemwise=true)
        //   params (all varchar): @Course, @REGU, @Sem, @NoOfToppers, @Branch, @Caste, @Gender, @WithRv
        //   Returns: same columns as PROC_TOPPERSLIST_NEW but ranked within each semester
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 169701 "[PROC_TOPPERSLIST_SemWise]"
        //   BAL method: Get_Toppers_List_SemWise
        //
        // Batch dropdown (DDLBatch) — raw SQL (reuses BackLogs batch SQL pattern):
        //   SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //   FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU
        //   BAL method: Load_Batch / DDLBatch_SelectedIndexChanged → also triggers LoadSem
        //
        // Max semester loading (DDLBatch_SelectedIndexChanged) — raw SQL:
        //   SELECT DISTINCT max(SEM) FROM tbl_sh WHERE REGU = @REGU
        //   Confirmed: DataAccessLayer.dll UTF-16LE fragment before [PROC_TOPPERSLIST_NEW]
        //   BAL method: LoadSem → auto-fills txtSemTo with max semester for selected batch
        // ------------------------
        PROC_TOPPERSLIST_NEW,
        PROC_TOPPERSLIST_SemWise,

        // ------------------------
        // Results — Student Result (Results/StudentResult.aspx)
        // SP_PASSEDLIST_NEW   : passed papers for a student (RBPassedList radio → addingPassedPapers)
        //   params: @RegNo (varchar — 1 param confirmed by byte pattern 0x27 0x00 0x01 after SP name)
        //   Returns: SEM, PCODE, PNAME, CR, GR, EXAMMY (Last Attempt)
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 158822 "SP_PASSEDLIST_NEW '"
        //   BAL method: StudentPassedList (RBPassedList_CheckedChanged → addingPassedPapers)
        //
        // SP_FAILEDLIST_NEW   : failed papers for a student (RBFailedList radio → addingFailedPapers)
        //   params: @RegNo (varchar — 1 param confirmed by byte pattern)
        //   Returns: SEM, PCODE, PNAME, CR, GR, EXAMMY (Last Attempt)
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 161053 "[SP_FAILEDLIST_NEW]"
        //   BAL method: StudentFailedList (RBFailedList_CheckedChanged → addingFailedPapers)
        //
        // All-papers (Rbtn_allFP / adding_passe_FailedPapers) — raw SQL on TBL_MRKMEMO:
        //   SELECT REGNO, SEM, SNAME, GRP BRANCH, PCODE, PNAME, CR, GR, PAPRES, SGPA, REGSUP
        //   FROM TBL_MRKMEMO WHERE REGNO = @RegNo [AND EXAMMY = @ExamMY]
        //   Confirmed: DataAccessLayer.dll UTF-16LE fragment before [SP_FAILEDLIST_NEW]
        //     "SELECT REGNO, SEM, SNAME, GRP BRANCH,PCODE,PNAME, CR,GR,PAPRES, SGPA,REGSUP
        //      FROM TBL_MRKMEMO # WHERE EXAMMY = '/'"
        //   CBCurrentMonthYear adds AND EXAMMY = @ExamMY filter; unchecked = all exam periods
        //
        // SPM_STUDENT_DETAILS  (reused — already in enum, loads txtStudentName/txtCourse/txtGRP)
        // PROC_SGPA_AVERAGE    (reused — already in enum, loads gvSGPA_CGPA: SEM,SGPA,CGPA,TCR,SCR)
        // ------------------------
        SP_PASSEDLIST_NEW,
        SP_FAILEDLIST_NEW,

        // Results — Subjectwise Failed List (Results/SubjectwiseFailedList.aspx)
        // SP_SubJ_FAILEDLIST_NEW : view grid data (btnView_Click → Load_Grid_SubwiseFailedList)
        //   params (all varchar): @Course, @REGU, @Sem, @Branch, @PCode, @ExamMY
        //   @ExamMY = '' → no date filter (CBCurrentMonthYear unchecked)
        //   @ExamMY = 'NOV2024' → filter to that month/year (CBCurrentMonthYear checked)
        //   Confirmed: DataAccessLayer.dll UTF-16LE offset 162123 "[SP_SubJ_FAILEDLIST_NEW]"
        //   BAL method: Load_Grid_SubwiseFailedList (Bal_Reports_Results)
        //   Dropdown cascade: batch→sem→branch→subject (all raw SQL on tbl_sh/TBL_COURSE)
        SP_SubJ_FAILEDLIST_NEW,

        // Results — RV Closing Date(s) (Results/RvClosingDates.aspx)
        // SP_RV_CLOSINGDATES_LIST : load gvRvCloseDate on Page_Load
        //   params: none (loads all RV closing date records)
        //   Returns: REGULATION, COURSE, EXAMMY, SEM, RV_CLOSEDATE, RV_CDATE_SUP
        //   BAL method: rvClosingDates_List (BAL_RVRegistrations)
        //   Confirmed: DataAccessLayer.dll UTF-16LE sequence "SP_RV_CLOSINGDATES_LIST"
        //
        // SP_RV_CLOSINGDATES_Update : update closing dates for one row (btnSave_Click)
        //   params (all varchar): @Regulation, @Course, @ExamMY, @Sem, @RV_CLOSEDATE, @RV_CDATE_SUP
        //   BOL properties: RvRegCloseDt (txtReg_DATE), RvSupCloseDt (txtSup_DATE)
        //   BAL method: rvClosingDates_Update (BAL_RVRegistrations)
        //   Confirmed: DataAccessLayer.dll UTF-16LE sequence "SP_RV_CLOSINGDATES_Update"
        SP_RV_CLOSINGDATES_LIST,
        SP_RV_CLOSINGDATES_Update,

        // Results — NBA SGPA and CGPA Data (Results/RegnoWiseSgpaCgpaList.aspx)
        // PROC_GET_CGPA_SGPA_EXCEL : load grid — Regular AND Supply exams considered
        //   params (all varchar): @Course, @REGU, @Sem
        //   BAL method: loadgridview (Bal_Reports_Results → get_rgnowise_SgpaCgpa_Avg)
        //   Confirmed: DataAccessLayer.dll UTF-16LE "PROC_GET_CGPA_SGPA_EXCEL"
        //
        // PROC_SGPA_AVERAGE : load grid — Regular exams only (Supply NOT considered)
        //   params (all varchar): @Course, @REGU, @Sem
        //   BAL method: loadgridview_WithOutSupply (Bal_Reports_Results → get_rgnowise_SgpaCgpa_Avg_RegularOnly)
        //   Confirmed: DataAccessLayer.dll UTF-16LE "PROC_SGPA_AVERAGE"
        PROC_GET_CGPA_SGPA_EXCEL,
        // PROC_SGPA_AVERAGE already declared above (line 669) — duplicate removed

        // Results — NBA CGPA Year Wise (Results/CGPA_YearWise.aspx)
        // sp_cgpa_excel : Excel download (btnDownLoad_Click)
        //   params: @ExamMY, @Batch (2 params confirmed from App_Web_m2jhophz.dll UTF-16 "sp_cgpa_excel '','")
        //   BAL: BAL_CGPA_Yearwise (bal), DAL: DAL_CGPA_Yearwise, BOL: BOL_CGPA_Yearwise
        //   Confirmed: App_Web_m2jhophz.dll UTF-16LE "sp_cgpa_excel '','  CGPA_YearWise"
        sp_cgpa_excel,

        // Results — Marks Data Internal & External (Results/MarksData_Int_Ext.aspx)
        // sp_Export_shdata : Export SH data for university formats 1–3 and 5
        //   params: @Regu, @ExamMY, @Sem, @Type
        //   Type values: 'CourseComplete' | 'stddata' | 'Regnowise' | 'JNTUK CE'
        //   BAL: Bal_Reports_Source (bal_RRS) → Loading_Marks_Int_Ext / Loading_Marks_Int_Ext_Checked
        //   DAL: Dal_Reports_Source → Export_shdata
        //   Confirmed: DataAccessLayer.dll UTF-16LE "sp_Export_shdata  '"
        sp_Export_shdata,

        // PROC_EXPORT_RES_DATA : Export result data — Format 4 (V1, RV, V3 Month & Year Wise)
        //   params: @Regu, @ExamMY  (2 params confirmed — DataAccessLayer.dll "[PROC_EXPORT_RES_DATA] '','")
        //   DAL: Dal_Reports_Source → Export_marksdata
        //   Confirmed: DataAccessLayer.dll UTF-16LE "[PROC_EXPORT_RES_DATA] '"
        PROC_EXPORT_RES_DATA,

        // Results — Credit Secured (Results/Credits_NextSem.aspx)
        // SP_Credit_Secured : export credit secured data (btnCredit_Click)
        //   params: @REGU (varchar), @Noofcredits (numeric), @Branch (varchar), @ExamMY (varchar)
        //   Page: "Credit Secured" — students with credits < threshold
        //   BAL: Bal_Reports_Results → Get_CreditList; DAL: Dal_Reports_Results
        //   Confirmed: DataAccessLayer.dll UTF-16LE "SP_Credit_Secured " + "',  " + " , "
        SP_Credit_Secured,

        // Results — Total Credit Secured (Results/RegnoWiseTotalCredits.aspx)
        // SP_Credit_Secured_Total : export total credit secured data (btnCredittotal_Click)
        //   params: @REGU (varchar), @Sem (varchar), @Branch (varchar), @ExamMY (varchar)
        //   BAL: Bal_Reports_Results → Get_CreditList_total; DAL: Dal_Reports_Results
        //   Confirmed: DataAccessLayer.dll UTF-16LE "SP_Credit_Secured_Total "
        SP_Credit_Secured_Total,

        // Results — OMR Number Update (Results/OmrNumberUpdate.aspx)
        // PROC_REGNOVSOMR : load OMR grid for a given regno (txtregno_TextChanged → LoadOmrGrid)
        //   params: @REGNO (varchar)
        //   returns: aSHID, PCode, TempCode, PName, OMRNUMBER, SNAME, SEM
        //   BAL: BAL_StudentWiseMasterCreation → Get_loadOmrNumUpdate
        //   Confirmed: DataAccessLayer.dll UTF-16LE "[PROC_REGNOVSOMR]" near ASHID context
        PROC_REGNOVSOMR,

        // Results — Result Sheet Excel Export (Results/EXCEL_GALLY.aspx)
        // PROC_EXCEL_GALLY : export result sheet to Excel (btnExcelExort_Click)
        //   params: @Course, @Regu, @Sem, @Branch, @REGSUP
        //   BAL: BAL_CGPA_Yearwise → get_EXCEL_GALLY; DAL method: Load_Sems_Data area
        //   Confirmed: DataAccessLayer.dll UTF-16LE "PROC_EXCEL_GALLY  '"
        PROC_EXCEL_GALLY,

        // Results — Result Sheet Excel Export (Results/EXCEL_GALLY.aspx)
        // PROC_GETfiledasubwithmarks : export backlogs (failed subjects with marks) to Excel
        //                              (btnBacklogsExcelExport_Click)
        //   params: @Course, @Regu, @Sem, @Branch, @REGSUP
        //   Confirmed: DataAccessLayer.dll UTF-16LE "PROC_GETfiledasubwithmarks  '"
        PROC_GETfiledasubwithmarks,

        // ------------------------
        // OD Data — Gracing (ODLIST_JBIET.aspx — chkgracing / Load_Btech_OD_AddGracing_R16)
        // PROC_GRACING_GRAFTING        : applies gracing to OD students (main update)
        //   params (6): @Course, @REGU, @BRANCH, @EXAMMY, @UserID, @Reg_Letrl
        //   Confirmed: DataAccessLayer.dll UTF-16LE sep=0x45 (same as PROC_MODERATION_REG_SEM_GRP_PAP=6 params)
        // PROC_GRACING_GRAFTING_MRK_UPDATE : updates marks for graced OD students
        //   params (6): @Course, @REGU, @BRANCH, @EXAMMY, @UserID, @Reg_Letrl
        //   Confirmed: DataAccessLayer.dll UTF-16LE sep=0x43
        // PROC_GRACING_GRAFTING_SH_UPDATE  : updates TBL_SH exam-date for graced OD students
        //   params (7): @Course, @REGU, @BRANCH, @EXAMMY, @UserID, @Reg_Letrl, @EDATE ('01-'+EXAMMY)
        //   Confirmed: DataAccessLayer.dll UTF-16LE sep=0x19 (same as PROC_MODERATION_AND_MCNT=7 params)
        //              template fragment shows ' , '''''01-<param>''''' → @EDATE constructed in caller
        // ------------------------
        PROC_GRACING_GRAFTING,
        PROC_GRACING_GRAFTING_MRK_UPDATE,
        PROC_GRACING_GRAFTING_SH_UPDATE,

        // ------------------------
        // University Data — Export Marks / Award Degree Formats
        // PROC_EXPORT_MARKSDATA : exports marks data for university formats
        //   params (6): @Course, @REGU, @Sem, @RegSup, @ExamMY, @IsRv
        //   Used by: University_SubjectData.aspx, University_CD_Data.aspx,
        //            University_Formate_Data.aspx, UniversityData.aspx (format buttons)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap sep=0x41 after SP name
        //   BAL methods: Export_Data, Univeristy_Formate_Data, Export_marksdata
        // PROC_AWARD_DEGREE_UNIVERSITY : generates PC/award degree university format
        //   params (5): @Course, @REGU, @BRANCH, @ExamMY, @Reg_Letrl
        //   @Reg_Letrl: 'R' = Regular, 'L' = Lateral (ChkLateral)
        //   Used by: University_PC_Formate.aspx → Get_Univeristy_Formate_R18 / Get_Univeristy_Formate_Gracing
        //   Confirmed: DataAccessLayer.dll UTF-16LE sep=0x2D after SP name
        //              (same sep group as PROC_OMRNUM_UPDATE_Get = 5 params)
        //   BAL methods: Get_Univeristy_Formate_R18, Get_Univeristy_Formate_Gracing, UniversityData
        // ------------------------
        PROC_EXPORT_MARKSDATA,
        PROC_AWARD_DEGREE_UNIVERSITY,

        // ------------------------
        // Training & Placement Data (Results/TandPdata.aspx)
        // sp_t_and_p_data : exports T&P data for a batch (btnDownLoad_Click)
        //   params (1): @REGU (varchar — REGU value from ddlbatch)
        //   Page: single control ddlbatch (REGU selector); Course from master page session
        //   BAL/DAL: codebehind in App_Web_m2jhophz.dll (iCampus_Results_TandPdata)
        //   Confirmed: App_Web_m2jhophz.dll UTF-16LE exec template
        //     "sp_t_and_p_data '" + sep=0x13 → 1 param (no comma separator before sep)
        //     Cross-checked against sp_cgpa_excel "',' " (2-param pattern in same DLL)
        //   Batch dropdown: inline SQL on TBL_COURSE (COURSE param from session)
        // ------------------------
        sp_t_and_p_data,

        // ------------------------
        // Grace Eligible Data (Results/Grace_Eligible_Data.aspx)
        // proc_load_audit_sem : loads Semester dropdown (Page_Load cascade)
        //   params (2): @Course, @REGU
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap
        //     template "proc_load_audit_sem '" — string-concat DAL pattern
        //     sequence: Proc_SubjectData_LoadSem → proc_load_audit_sem → [proc_Grace_Data]
        // proc_Grace_Data : fetches grace eligible student data (btnGetData_Click)
        //   params (4): @Course, @REGU, @Sem, @IsLE
        //   @IsLE: 1 = Lateral Entry (ChkIsLE checked), 0 = all students
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap
        //     template "[proc_Grace_Data] '" — string-concat DAL pattern
        //   BAL: Bal_Reports_Results (bal_RR), DAL method: Load_GraceData
        //   Confirmed: App_Web_m2jhophz.dll ASCII strings
        //     "ChkIsLE\x00btnGetData\x00bal_RR\x00btnGetData_Click"
        // ------------------------
        proc_load_audit_sem,
        proc_Grace_Data,

        // ------------------------
        // V3 Data / RV2 Data (Results/V3_Data.aspx)
        // PROC_DATAFOR_V3 : fetches V3 (second revaluation) data (btnView_Click)
        //   params (5): @Course, @Regulation, @ExamMY, @Sem, @DiffMarks
        //   Returns all V3 marks data (AutoGenerateColumns grid — columns vary)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap — exec template "PROC_DATAFOR_V3 '"
        // PROC_DATAFOR_V3_READMIT : same but for readmit students (chkReadmit_CheckedChanged)
        //   params (6): @Course, @Regulation, @ExamMY, @Sem, @DiffMarks, @ReadmitReg
        //   @ReadmitReg: regulation for readmit students (txtreadmireulation from modal popup)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap — exec template "PROC_DATAFOR_V3_READMIT '"
        // Sem dropdown: inline SQL (SELECT DISTINCT SEM FROM tbl_sh WHERE COURSE+REGULATION+EXAMMY)
        // ------------------------
        PROC_DATAFOR_V3,
        PROC_DATAFOR_V3_READMIT,

        // ------------------------
        // Course Percentage (Reports/CoursePercentage.aspx)
        // sp_PAP_PERCENT : fetches course-wise pass percentage data (regular / RV mode)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Returns: PAP_STAT rows (PCODE, PNAME, GRP, SEM, appeared, passed, % etc.)
        //   Crystal Report: CoursePercent.rpt (selection: {PAP_STAT.SEM} = @Sem)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 160036
        //   Used when: ChkIsrv NOT checked (regular) OR ChkIsrv checked (RV — same SP, DB flag)
        // SP_Pap_Percent_Sup : supply exam variant
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 160090
        //   Used when: Chkregsup checked
        // Sem dropdown: inline SQL SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        //   BAL: Branch_Wise_Course_Percentage_LoadSemesters (DataAccessLayer.dll ASCII offset 114866)
        //   BAL: CoursePercentage_and_Chart (DataAccessLayer.dll ASCII offset 116342)
        // ------------------------
        sp_PAP_PERCENT,
        SP_Pap_Percent_Sup,

        // ------------------------
        // Failed in Sem Result, Passed in Subjects (Reports/Failed_inResult_Passed_inSub.aspx)
        // SP_SubJ_FAILEDLIST_NEW : students who failed overall but passed in individual subjects
        //   params (2): @Course, @ExamMY
        //   Crystal Report: FailedResult.rpt (label "Passed_List Subjectwise")
        //   CR selection: {tbl_SH.PMARKS} in ['ab','sm'] — absent/subject marked filter
        //   No user controls on ASPX — loads on Page_Load with session Course+ExamMY
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162119
        //   BAL class: Adjacent to SP_FAILEDLIST_NEW (subjectwise variant)
        //   NOTE: SP_SubJ_FAILEDLIST_NEW is also declared above (line 804) for SubjectwiseFailedList
        //         Reuse that enum entry — no duplicate needed here
        // ------------------------
        // (duplicate removed — use existing SP_SubJ_FAILEDLIST_NEW above)

        // ------------------------
        // Grade Card / Result Grade Sheet (Reports/ResultGradeSheet.aspx)
        // SP_REP_GRADE_CHKLIST : regular result grade checklist
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Used when: chkRv=false, ChkIsreadmitresult=false
        // SP_REP_GRADE_CHKLIST_Readmit : for readmit students
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Used when: ChkIsreadmitresult=true, chkRv=false
        // SP_REP_GRADE_CHKLIST_RV : after revaluation
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Used when: chkRv=true, ChkIsreadmitresult=false
        // SP_REP_GRADE_CHKLIST_RV_Readmit : RV + readmit
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Used when: chkRv=true, ChkIsreadmitresult=true
        //   Crystal Report: GradeCheckList.rpt
        //   Title strings: 'Results Sheet', 'Revaluation Results Sheet',
        //                  'Results Sheet (Re-admitted)', 'Revaluation Results Sheet (Re-admitted)'
        //   Confirmed: App_Web_oxqewfcs.dll UTF-16LE US heap offsets 48901, 48841, 49063, 48993
        //   ALSO: SP_GRADESHEET_RESULT_RELEASE (DataAccessLayer offset 147940) — admin result release SP
        // ------------------------
        SP_REP_GRADE_CHKLIST,
        SP_REP_GRADE_CHKLIST_Readmit,
        SP_REP_GRADE_CHKLIST_RV,
        SP_REP_GRADE_CHKLIST_RV_Readmit,

        // ------------------------
        // MTECH CMM (Reports/MTECHCMM.aspx)
        // SP_CMM : Consolidated Marks Memo for all courses
        //   params (3): @Course, @ExamMY, @Regu
        //   Crystal Report: MTECHCMM.rpt (no user filter controls)
        //   No controls in ASPX — loads on Page_Load with session data
        //   BTECHCMM uses variants: SP_CMM_R11_PG (PG/R11), SP_CMM_R18, SP_CMM_AddGracing, SP_CMM_AddGracing_R16
        //   MTECHCMM uses SP_CMM directly (no batch/branch/gracing UI)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offsets 159387/159411
        //   BAL: MTECHCMM/BTECHCMM/MBACMM → Load_Btech_CMM (DataAccessLayer.dll ASCII offset 102636)
        // ------------------------
        SP_CMM,

        // ------------------------
        // Branch Wise Percent (Reports/BranchWisePercent.aspx)
        // sp_COURSE_STAT : branch-wise pass percentage per semester
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @IsRv ('N'=regular, 'Y'=after RV/SM/GR)
        //   Crystal Report: BranchWisePercent.rpt ('Before RV/SM/GR' / 'After RV/SM/GR' title)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159742
        //     adjacent pattern: "sp_COURSE_STAT '" + "', 'N','"  → IsRv flag literal
        //   BAL: BRANCHWISE_PERCENT_Chart (DataAccessLayer.dll ASCII offset 116317)
        //   Sem dropdown: inline SQL (see Branch_Wise_Course_Percentage_LoadSemesters)
        // ------------------------
        sp_COURSE_STAT,

        // ------------------------
        // Passed Result (Reports/PassedResult.aspx)
        // SP_PASSEDLIST_NEW : passed students list for a semester
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Crystal Report: PassedResult.rpt (label "PassedResultList")
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158822
        //   BAL: PassedResult (DataAccessLayer.dll ASCII offset 115765)
        //   Sem dropdown: inline SQL SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        //     FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158863
        // ------------------------
        // SP_PASSEDLIST_NEW already declared above (line 793) — duplicate removed

        // ------------------------
        // CGC All Programmes / BTECH CMM (Reports/BTECHCMM.aspx — title "CGC Report")
        // SP_CMM_R11_PG   : CMM for PG students with R11 regulation
        //   params (3): @Course, @ExamMY, @Regu
        // SP_CMM_R18      : CMM for R18 regulation
        //   params (3): @Course, @ExamMY, @Regu
        // SP_CMM_AddGracing : CMM with gracing applied
        //   params (6): @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        // SP_CMM_AddGracing_R16 : CMM with gracing for R16 regulation
        //   params (6): @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        //   Controls: ChkLateral, chkgracing, ddlBatch (AutoPostBack), ddlBranch, txtRegno (H.T.No), btnView
        //   SP selection: isGracing+R16→SP_CMM_AddGracing_R16, isGracing→SP_CMM_AddGracing,
        //                 R18→SP_CMM_R18, R11+PG→SP_CMM_R11_PG, default→SP_CMM
        //   Non-gracing SPs reuse SP_CMM (3 params, already in enum)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 161251
        //     adjacent sequence: SP_CMM_R11_PG, SP_CMM_R18, SP_CMM, SP_CMM_AddGracing, SP_CMM_AddGracing_R16
        //   BAL: BTECHCMM → Load_Btech_CMM_AddGracing, Load_Btech_CMM_AddGracing_R16
        //        (BusinessAccessLayer.dll ASCII offsets 41586/35178)
        //   Batch loading: inline SQL SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //     '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //     FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        //   Branch loading: inline SQL SELECT DISTINCT GRP FROM TBL_COURSE
        //     WHERE COURSE=@Course AND REGU=@Batch
        // ------------------------
        SP_CMM_R11_PG,
        SP_CMM_R18,
        SP_CMM_AddGracing,
        SP_CMM_AddGracing_R16,

        // ------------------------
        // PC All Courses (Reports/PC_All_Courses.aspx)
        // proc_pc_rep_AllCourse : default PC report for all course types
        //   params (3): @Course, @ExamMY, @Regu
        // proc_pc_rep           : base BTECH PC report
        //   params (3): @Course, @ExamMY, @Regu
        // proc_pc_rep_R18       : PC report for R18 regulation
        //   params (3): @Course, @ExamMY, @Regu
        // proc_pc_rep_AddGracing : PC report with gracing
        //   params (6): @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        // proc_pc_rep_AddGracing_R16 : PC report with gracing for R16
        //   params (6): @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        //   Controls: chkgracing, ddlBatch (AutoPostBack), ddlBranch, txtRegno (H.T.No), btnView
        //   SP selection: isGracing+R16→proc_pc_rep_AddGracing_R16, isGracing→proc_pc_rep_AddGracing,
        //                 R18→proc_pc_rep_R18, default→proc_pc_rep_AllCourse
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 161643 (proc_pc_rep)
        //     offset 162903 (proc_pc_rep_AllCourse)
        //   BAL: Allcourses_Pc_R18, Load_Mtech_Pc_AddGracing, Load_Mtech_Pc_AddGracing_R16
        //        (BusinessAccessLayer.dll ASCII offsets 35208/41612)
        // ------------------------
        proc_pc_rep_AllCourse,
        proc_pc_rep,
        proc_pc_rep_R18,
        proc_pc_rep_AddGracing,
        proc_pc_rep_AddGracing_R16,

        // ------------------------
        // Result Check List (Reports/ResultCheckList.aspx)
        // Result Sheet - V1 (Marks) (Reports/ResultSheet.aspx)
        // Both pages share the same SPs; Crystal Report template differs (CheckList.rpt vs ResTR.rpt)
        // SP_REP_MRK_CHKLIST : marks-based result check list / result sheet
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        // SP_REP_MRK_CHKLIST_Readmit : same for re-admitted students
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Controls (ResultCheckList): ddlSem, rbtn1 (Check List-I), rbtn2 (Check List-II),
        //     ChkIsreadmitresult, btnView, modal txtreadmitReulation
        //   Controls (ResultSheet): ddlSem, chkRv (HIDDEN), ChkIsreadmitresult, btnview,
        //     modal txtreadmitReulation
        //   SP selection: isReadmit=true → SP_REP_MRK_CHKLIST_Readmit, else → SP_REP_MRK_CHKLIST
        //   checkListType (1=Check List-I, 2=Check List-II) → frontend Crystal Report title only
        //   Sem dropdown: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regu
        //     AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        //   Confirmed: App_Web_oxqewfcs.dll UTF-16LE offset 44424 (ResultCheckList)
        //              App_Web_gp3pforx.dll UTF-16LE offset 205422 (ResultSheet)
        //   Crystal reports: CheckList.rpt, CheckList_r17.rpt, CheckList_r14.rpt; ResTR.rpt
        // ------------------------
        SP_REP_MRK_CHKLIST,
        SP_REP_MRK_CHKLIST_Readmit,

        // ------------------------
        // Result Sheet - V1 & RV (Grades) (Reports/ResultSheet_Grafting.aspx)
        // SP_REP_MRK_CHKLIST_GRFLAG : marks result sheet with grafting (V1+RV combined)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        // SP_REP_MRK_CHKLIST_Readmit_GRFLAG : same for re-admitted students
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Controls: ddlSem, chkRv (HIDDEN), ChkIsreadmitresult, btnview,
        //     modal txtreadmitReulation
        //   SP selection: isReadmit=true → SP_REP_MRK_CHKLIST_Readmit_GRFLAG, else → SP_REP_MRK_CHKLIST_GRFLAG
        //   Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 202774
        //   Crystal report: ResTR_GRFLAG.rpt
        // ------------------------
        SP_REP_MRK_CHKLIST_GRFLAG,
        SP_REP_MRK_CHKLIST_Readmit_GRFLAG,

        // ------------------------
        // Result Sheet - Subject Moderation (Reports/ResultSheet_Moderation.aspx)
        // SP_REP_MRK_CHKLIST_SMFLAG : marks result sheet with subject moderation flag
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        // SP_REP_MRK_CHKLIST_Readmit_SMFLAG : same for re-admitted students
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Controls: ddlSem, chkRv (HIDDEN display:none), ChkIsreadmitresult, btnview,
        //     modal txtreadmitReulation (readmit regulation)
        //   SP selection: isReadmit=true → SP_REP_MRK_CHKLIST_Readmit_SMFLAG, else → SP_REP_MRK_CHKLIST_SMFLAG
        //   Sem SQL: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regu
        //     AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        //   Crystal report: ResTR_SMFLAG.rpt
        //   Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x2e10b:
        //     "ResTR_SMFLAG.rpt" → "[SP_REP_MRK_CHKLIST_Readmit_SMFLAG]" → "[SP_REP_MRK_CHKLIST_SMFLAG]"
        // ------------------------
        SP_REP_MRK_CHKLIST_SMFLAG,
        SP_REP_MRK_CHKLIST_Readmit_SMFLAG,

        // ------------------------
        // Tabulation Register (Reports/TabulationRegister.aspx)
        // SP_TABULATION_REGISTER : regular tabulation register
        //   params (5): @Course, @ExamMY, @Regu, @Branch, @RegNo
        // SP_TABULATION_REGISTER_Readmit : for re-admitted students
        //   params (6): @Course, @ExamMY, @Regu, @Branch, @RegNo, @ReadmitRegu
        //   Controls: ddlBatch (AutoPostBack→loads ddlBranch), ddlBranch, txtRegno (H.T.No),
        //             cmbExamMY, ChkIsreadmitresult, btnView, modal txtreadmitReulation
        //   Crystal reports: TabulationRegister_btech.rpt, TabulationRegister_mca.rpt, TabulationRegister_mba.rpt
        //   Batch SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //     '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //     FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        //   Branch SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP
        //   ExamMY SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS WHERE COURSE=@Course
        //     and REGULATION=@Regu ORDER BY AEXAMID DESC
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 161101/161161
        // ------------------------
        SP_TABULATION_REGISTER,
        SP_TABULATION_REGISTER_Readmit,

        // ------------------------
        // RV Reports — Check Lists & Result Sheet (Reports/RvMarksCheckList.aspx)
        // PROC_RV_REPDATA : RV marks report (Check List-I, Check List-II, or Result Sheet)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        // PROC_RV_REPDATA_Readmit : same for re-admitted students
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        //   Controls: rbtn1 (Check List-I), rbtn2 (Check List-II), rbtnRSheet (Result Sheet),
        //             ddlSemester, ChkIsreadmitresult, btnExport
        //   reportType: 1=Check List-I, 2=Check List-II, 3=Result Sheet → Crystal Report title only; same SP
        //   Sem SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892/159934
        // ------------------------
        PROC_RV_REPDATA,
        PROC_RV_REPDATA_Readmit,

        // ------------------------
        // SGPA & CGPA H.T.No. wise (Reports/SGPA_CGPA_Regnowise.aspx)
        // PROC_SGPA_AVERAGE_OnlyRegular : SGPA/CGPA without revaluation (ChkRv=false)
        //   params (4): @Course, @ExamMY, @Regu, @Branch
        // PROC_SGPA_AVERAGE (already in enum) : SGPA/CGPA with revaluation (ChkRv=true)
        //   params (4): @Course, @ExamMY, @Regu, @Branch
        //   Controls: ddlBatch (AutoPostBack→loads ddlBranch), ddlBranch, cmbExamMY, ChkRv, btnView
        //   Crystal Report: SGPA.rpt; subtitle "(With Revaluation)" or "(Without Revaluation)"
        //   Branch SQL: SELECT DISTINCT GRP FROM tbl_SH WHERE COURSE=@Course Order by GRP
        //   ExamMY SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS WHERE COURSE=@Course
        //     and REGULATION=@Regu ORDER BY AEXAMID DESC
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 165428 (PROC_SGPA_AVERAGE) / 166874 (OnlyRegular)
        //   Note: Distinct from existing RegnoWiseSgpaCgpaController (Sem-based, different page)
        // ------------------------
        PROC_SGPA_AVERAGE_OnlyRegular,
        PROC_SGPA_Report,

        // ------------------------
        // Award of Class Branchwise (Reports/AwardofClassBranchwise.aspx)
        // PROC_GRADE_CNT : Crystal Report view (btnView_Click)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        // PROC_GRADE_CNT_EXCEL : Excel download (btnDownLoad_Click)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Controls: ddlbatch (AutoPostBack→loads nothing on batch change, used for REGU),
        //             cmbExamMY (AutoPostBack→cmbExamMY_SelectedIndexChanged→loads ddlSemester),
        //             ddlSemester, ChkIsrv (HIDDEN display:none), btnView, btnDownLoad
        //   Crystal Report: AwardofClassBranchwise.rpt (Excel filename: AwardofClassBranchwise.xlsx)
        //   Subtitle: "RESULTS GRADE ANALYSIS" (from App_Web_gp3pforx.dll offset 191010)
        //   BAL method: AwardofClassCnt (DataAccessLayer.dll ASCII offset 115859)
        //   Batch SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //     '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //     FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        //   Sem SQL (cmbExamMY_SelectedIndexChanged):
        //     SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        //     WHERE COURSE=@Course AND EXAMMY=@ExamMY
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162245 (PROC_GRADE_CNT)
        //              App_Web_gp3pforx.dll UTF-16LE offset 191010 (PROC_GRADE_CNT_EXCEL)
        // ------------------------
        PROC_GRADE_CNT,
        PROC_GRADE_CNT_EXCEL,

        // ------------------------
        // Exam Fee Collection Report (Reports/ExamFeeCollection.aspx) — Sem+Branch based
        // SPR_ExamFee_Collection : load Crystal Report (btnview_Click)
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @Branch
        //   Controls: ddlSemester (loaded on Page_Load), ddlBranch (loaded on Page_Load,
        //             validation commented out = optional filter), chkIsPrint, btnview
        //   Note: Distinct from Pre-Exams/ExamFeeCollection.aspx (date-range, SPM_EXAMFEE_COLLECTION)
        //   Sem SQL (Page_Load): SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //     FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        //   Branch SQL (Page_Load): SELECT DISTINCT GRP FROM TBL_COURSE
        //     WHERE COURSE=@Course ORDER BY GRP
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162019
        // ------------------------
        SPR_ExamFee_Collection,

        // ------------------------
        // NBA — Award Degree List (Reports/AwardDegree.aspx)
        // PROC_AWARD_DEGREE : Crystal Report view (cmbExamMY_SelectedIndexChanged auto-loads)
        //                     + Excel download (btnDownLoad_Click)
        //   params (2): @Regu, @ExamMY
        //   Controls: ddlbatch (Batch/REGU, AutoPostBack → ddlbatch_SelectedIndexChanged),
        //             cmbExamMY (AutoPostBack → cmbExamMY_SelectedIndexChanged),
        //             btnView (hidden, display:none on TD), btnDownLoad (Export Excel, visible)
        //   Crystal Report: AwardDegreeList.rpt
        //   Excel filename: AwardDegreeList_[regu].xlsx
        //   Note: No Course param in SP — works across all courses
        //   Batch SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //     '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //     FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        //   ExamMY SQL (ddlbatch_SelectedIndexChanged):
        //     SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS
        //     WHERE COURSE=@Course AND REGULATION=@Regu ORDER BY AEXAMID DESC
        //   Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x31a6e context:
        //     "AwardDegreeList.rpt" + "PROC_AWARD_DEGREE 'x','y'" (2 param placeholders)
        // ------------------------
        PROC_AWARD_DEGREE,

        // ------------------------
        // Branch wise Course Section Percent (Reports/BranchwiseCourseSecPercent.aspx)
        // PROC_CLASSWISE_COUNT : Crystal Report view (btnView_Click)
        //   params (5): @Course, @ExamMY, @Regu, @Sem, @IsRv
        //   @IsRv: 'N' = regular, 'Y' = after RV/SM (ChkIsrv checkbox)
        //   Controls: ddlSemester (Page_Load, no AutoPostBack), ChkIsrv (RV toggle),
        //             btnView, btnDownLoad (hidden, Visible=false)
        //   Crystal Report: ClassWiseCnt.rpt
        //   Sem SQL (Page_Load): SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //     FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        //   BAL method: BranchwiseCourseSecPercent_and_Chart (DataAccessLayer.dll ASCII offset 0x1c6c0)
        //   Confirmed: App_Web_gp3pforx.dll UTF-16LE: "ClassWiseCnt.rpt" near
        //     "{PROC_CLASSWISE_COUNT.SEM}" Crystal Report formula field reference
        // ------------------------
        PROC_CLASSWISE_COUNT,

        // ------------------------
        // Branch wise Section Percent (Reports/BranchWiseSectionPercentage.aspx)
        // sp_PAP_SEC_PERCENT : Crystal Report view (btnView_Click)
        //   params (4): @Course, @ExamMY, @Regu, @Sem
        //   Controls: ddlSemester (Page_Load, no AutoPostBack), NO ChkIsrv,
        //             btnView, btnDownLoad (hidden, Visible=false)
        //   Sem SQL (Page_Load): SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //     FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        //   BAL method: BranchWiseSectionPercentage_and_Chart (DataAccessLayer.dll ASCII offset 0x1c691)
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 0x27132:
        //     "sp_PAP_SEC_PERCENT ..." in the SP lookup table
        // ------------------------
        sp_PAP_SEC_PERCENT,

        // ------------------------
        // Transcript (Reports/Transcript.aspx)
        // SP_Transcript_New : Transcript mode (chkgcard unchecked)
        //   params (6): @Course, @ExamMY, @Regu, @Sem, @Branch, @RegNo
        //   Controls: ddlSemester, ddlBranch, txtRegno (H.T.No), chkgcard (MarksMemo, default checked=true)
        //   chkgcard=true  → SP_MRK_MEMO (already in enum, 8 params)
        //   chkgcard=false → SP_Transcript_New (6 params)
        //   Crystal Reports (course-dependent): Transcript_R14_MBAMCA.rpt, Transcript.rpt,
        //                                       Transcript_Gr_btech.rpt, Transcript_Gr.rpt
        //   BAL method: loadingMarksMemo (chkgcard=true), Transcript mode (chkgcard=false)
        //   Sem SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //     FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        //   Branch SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 0x26dd5:
        //     "[SP_Transcript_New] ..." adjacent to "[SP_MRK_MEMO]" in method sequence
        // ------------------------
        SP_Transcript_New,

        // ------------------------
        // Individual MarksMemo / GradeCard (Reports/Individual_Marksmemo.aspx)
        // SP_MRK_MEMO_REGNO : per-regno marks memo (both MarksMemo & GradeCard modes)
        //   params (7): @REGULATION, @EXAMMY, @Course, @SEMESTER, @RV='N', @BRANCH, @REGNO
        //   @RV is hardcoded 'N' (no revaluation) — not exposed as API param
        //   Controls: ddlSemester, ddlBranch, txtRegno
        //   chkgcard (frontend-only, same SP for both):
        //     Checked  (MarksMemo) → Crystal: MarksMemo_Regno.rpt
        //     Unchecked (GradeCard) → Crystal: GradeCard_btech_Regno.rpt / GradeCard_Regno.rpt
        //   Sem SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM tbl_sh
        //            WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        //   Branch SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        //   Student-info SQL (auto-fill): SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap:
        //     "[SP_MRK_MEMO_REGNO] 'ᔁ ,'N',  '⬁" (offset ~0x26e6d)
        //     adjacent to "[SP_MRK_MEMO]" and "[SP_Transcript_New]" sequences
        // ------------------------
        SP_MRK_MEMO_REGNO,

        // ------------------------
        // Transfer Certificate Issuing (Reports/TC_Issue.aspx)
        // Proc_TC_Issue : issue/generate Transfer Certificate (INSERT/EXEC)
        //   Called as CommandType.StoredProcedure (not inline EXEC)
        //   Params (inferred from ASPX + App_Web binary):
        //     @RegNo, @Regulation, @Section, @SName, @FName, @MName,
        //     @DOB, @Gender, @Caste, @Email, @Mobile, @AadhaarNo,
        //     @MOLE1, @MOLE2, @Religion, @DateofAdmitted,
        //     @Scholar, @CourseComplete, @HigherEdu, @DateofLeave
        //   Student auto-load (txtRegNo_TextChanged / loadingTC_Issue):
        //     SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        //   chknoresult: when true → calls TC_issue_Noresult path (same SP, no result data)
        //   Crystal: Transfercertificate_JBIET.rpt
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap 0x28fae: "Proc_TC_Issue  '⼁"
        // ------------------------
        Proc_TC_Issue,

        // ------------------------
        // Study Certificate Issuing (Reports/SC_Issue.aspx)
        // Proc_SC_Issue : issue/generate Study Certificate (INSERT/EXEC)
        //   Called as CommandType.StoredProcedure (not inline EXEC)
        //   Params (inferred from ASPX + App_Web binary):
        //     @RegNo, @Regulation, @Section, @SName, @FName, @MName,
        //     @Conduct, @DOB, @Gender, @Caste, @Email, @Mobile, @AadhaarNo, @Religion
        //   Student auto-load (txtRegNo_TextChanged / loadingSC_Issue):
        //     SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        //   Crystal: Studycertificate_JBIET.rpt
        //   Confirmed: DataAccessLayer.dll UTF-16LE US heap 0x28f8c: "Proc_SC_Issue  '℁"
        //     adjacent to "Proc_TC_Issue" in method sequence
        // ------------------------
        Proc_SC_Issue,

        // =======================================================================
        //  BATCH 8 — University PC Format / RV Summary Report
        // =======================================================================

        // SP_Jntu_Award_JBIET : JNTUH University Provisional Certificate (default / non-gracing)
        //   Params (3): @Course, @ExamMY, @Regu
        //   Source: University_PC_Formate.aspx → Get_Univeristy_Formate / Get_Univeristy_Formate_R18
        //   Crystal: Btech_Pc.rpt / MCA_Pc.rpt / MBA_Pc.rpt / Mtech_Pc.rpt (chosen by frontend)
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x276af: "[SP_Jntu_Award_JBIET]  '✁"
        // ------------------------
        SP_Jntu_Award_JBIET,

        // SP_Jntu_Award_JBIET_AddGracing : University PC with gracing (non-R16)
        //   Params (5): @Course, @ExamMY, @Regu, @Batch, @Branch
        //   Source: University_PC_Formate.aspx → Get_Univeristy_Formate_Gracing (ChkGracing=true, regu≠R16)
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27aef: "[SP_Jntu_Award_JBIET_AddGracing]  '儁"
        // ------------------------
        SP_Jntu_Award_JBIET_AddGracing,

        // SP_Jntu_Award_JBIET_AddGracing_R16 : University PC with gracing + R16 regulation
        //   Params (5): @Course, @ExamMY, @Regu, @Batch, @Branch
        //   Source: University_PC_Formate.aspx → Get_Univeristy_Formate_Gracing (ChkGracing=true, regu contains R16)
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27aef: "[SP_Jntu_Award_JBIET_AddGracing_R16]  '儁"
        // ------------------------
        SP_Jntu_Award_JBIET_AddGracing_R16,

        // Proc_RV_Summary : RV Summary report data (Crystal Report)
        //   Params (2): @Regulation, @ExamMY
        //   Source: RV_Summery_Report.aspx → btnExport_Click (btnExport "Summary of RV Report")
        //           also used for btnRVExportExcel_Click ("Summary of RV Excel")
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_RV_Summary] '⼁"
        // ------------------------
        Proc_RV_Summary,

        // Proc_Supply_Summary : Supply Summary Excel export data
        //   Params (2): @Regulation, @ExamMY
        //   Source: RV_Summery_Report.aspx → ExcelExport_Click ("Summary of Supply Excel")
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_Supply_Summary] '㔁"
        //     adjacent to Proc_RV_Summary in same BAL class sequence
        // ------------------------
        Proc_Supply_Summary,

        // =======================================================================
        //  BATCH 9 — University SubjectData / University CD Data / Audit Course
        // =======================================================================

        // Proc_SubjectData_LoadSem : load semester dropdown for University SubjectData & CD_Data pages
        //   Params: @Course, @ExamMY  (inferred from set_Course/set_ExamMy + SubjectData_LoadSemesters pattern)
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27e5d: "Proc_SubjectData_LoadSem '⬁"
        //     appears immediately before proc_load_audit_sem in SP table
        // ------------------------
        Proc_SubjectData_LoadSem,

        // Proc_University_SubjectList : Subject List for University_SubjectData.aspx
        //   Params: @Course, @Regulation, @ExamMY, @Sem
        //   Source: University_SubjectData.aspx → btnSubjectList_Click → Load_SubjectList → set_Batch, set_Sem
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27d5b: "[Proc_University_SubjectList] '䜁"
        // ------------------------
        Proc_University_SubjectList,

        // Proc_University_SubjectData : Students Data for University_SubjectData.aspx
        //   Params: @Course, @Regulation, @ExamMY, @Sem, @RegSup
        //   Source: University_SubjectData.aspx → btnSubjectData_Click → Load_SubjectData → set_RegSup
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27e1f: "[Proc_University_SubjectData] '㔁"
        // ------------------------
        Proc_University_SubjectData,

        // Proc_University_SubjectList_CD : Subject List for University_CD_Data.aspx
        //   Params: @Course, @Regulation, @ExamMY, @Sem
        //   Source: University_CD_Data.aspx → btnSubjectList_Click → Load_SubjectList_CD → set_Batch, set_Sem
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27d5b: "[Proc_University_SubjectList_CD] '㤁"
        //     adjacent to Proc_University_SubjectList
        // ------------------------
        Proc_University_SubjectList_CD,

        // Proc_University_CD_Data : Students Data for University_CD_Data.aspx
        //   Params: @Course, @Regulation, @ExamMY, @Sem, @RegSup
        //   Source: University_CD_Data.aspx → btnStudentData_Click → Load_CD_Data → set_RegSup
        //   App_Web_gp3pforx.dll: set_Batch → set_Sem → Load_SubjectList_CD, then set_RegSup → Load_CD_Data
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x27de5: "[Proc_University_CD_Data] '䄁"
        // ------------------------
        Proc_University_CD_Data,

        // proc_load_audit_sem : already defined above (line ~958, near proc_Grace_Data)
        // Used by AuditCourseService for AuditCourse.aspx semester dropdown

        // PROC_AuditCourse_Data : main Audit Course data (btnview_Click / btnDownLoad_Click)
        //   Params: @Course, @Regu, @Sem, @Academic_year
        //   Source: AuditCourse.aspx (App_Web_gp3pforx.dll) → Audit_CourseData / Load_AuditCourse_Data
        //   Controls: ddlbatch (Regu from tbl_audit_course), ddlSemester (proc_load_audit_sem),
        //             ddlyear (Academic_year from tbl_audit_course), btnview, btnDownLoad
        //   Crystal: CrystalReportViewer2 (AuditCourse.rpt implied)
        //   Confirmed: DataAccessLayer.dll UTF-16LE 0x26c34: "[PROC_AuditCourse_Data] '✁"
        // ------------------------
        PROC_AuditCourse_Data
    }
}
