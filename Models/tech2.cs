//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LabMaintanance6.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tech2
    {
        public int action_id { get; set; }
        public int complain_id { get; set; }
        public string technicianName { get; set; }
        public string action_description { get; set; }
        public System.DateTime action_date { get; set; }
        public bool status { get; set; }
    
        public virtual Complain Complain { get; set; }
    }
}
