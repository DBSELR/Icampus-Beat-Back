using System;
using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    public class RoomAllotSemanticRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string DaySession { get; set; } = string.Empty;
        public string EDate { get; set; } = string.Empty; // 'yyyy-MM-dd' string as in your ASPX
        public string RoomNo { get; set; } = string.Empty;
        public string Regsup { get; set; } = "REG";
        public string Pcode { get; set; } = null;
        public int MaxNum { get; set; } = 10;
    }

    public class RoomAllotmentRow
    {
        public string RowCol { get; set; } = string.Empty; // format "1,1"
        public string Regno { get; set; } = string.Empty;
    }

    public class RoomAllotmentSaveRequest
    {
        public string Sem { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty; // DaySession
        public string ExamDate { get; set; } = string.Empty; // yyyy-MM-dd
        public string RoomNo { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        // Main payload: rows to insert
        public List<RoomAllotmentRow> RoomAllotment { get; set; } = new List<RoomAllotmentRow>();
    }

    public class RoomResetRequest
    {
        public string Sem { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty; // "yyyy-MM-dd" or ISO
        public string RoomNo { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
    }

    public class ApplyAllDatesRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
    }

    public class ExcelExportRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty; // yyyy-MM-dd
        public string DaySession { get; set; } = string.Empty; // Changed to string to match ASPX
        public string RoomNo { get; set; } = string.Empty;
    }
}
