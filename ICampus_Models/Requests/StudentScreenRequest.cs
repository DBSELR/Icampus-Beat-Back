using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_Models.Requests
{
    public class StudentScreenRequest
    {
        // used by all three APIs (Regno is required)
        public string Regno { get; set; } = string.Empty;

        // used by StudentGrades SP optionally (the SP signature in file accepts @exammy)
        public string ExamMy { get; set; } = string.Empty;
    }

}
