using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Read_Excel_OLEDB_MVC.Models
{

    public class clsNonUpdatedCCTV
    {
        
        public List<string> clsNonUpdatedName = new List<string>() ;
        public List<string> clsUpdatedName = new List<string>();
        public string clsNonUpdatedDate { get; set; }
        public string clsNonUpdatedIpAdd { get; set; }
    }
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
    }
}