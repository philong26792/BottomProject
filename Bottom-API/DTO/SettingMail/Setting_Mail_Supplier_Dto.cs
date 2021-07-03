
using System;

namespace Bottom_API.DTO.SettingMail
{
    public class Setting_Mail_Supplier_Dto
    {
        public string Factory { get; set; }
        public string Supplier_No { get; set; }
        public string Supplier_Name { get; set; }
        public string Contact_Persion { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Updated_By { get; set; }
        public string Subcon_ID { get; set; }
        public string Subcon_Name { get; set; }
        public DateTime? Updated_Time { get; set; }

        public Setting_Mail_Supplier_Dto()
        {
            this.Updated_Time = DateTime.Now;
        }
    }
}