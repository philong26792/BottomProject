using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class HP_Upload_Time_ie27_1_log
    {
        [Key][Column(Order = 0)]
        public string Factory_ID {get;set;}
        [Key][Column(Order = 1)]
        public string Version {get;set;}
        public DateTime? Upload_Time {get;set;}
        public string HP_User {get;set;}
        public string Update_By {get;set;}
        public DateTime? Update_Time {get;set;}
    }
}