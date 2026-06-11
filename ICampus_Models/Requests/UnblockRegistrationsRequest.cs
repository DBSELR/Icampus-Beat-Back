using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    public class UnblockRegistrationsRequest
    {
        public string Exammy { get; set; } = string.Empty;      // Required
        public List<string> Regnos { get; set; } = new List<string>(); // Required, list of registration numbers
    }
}

