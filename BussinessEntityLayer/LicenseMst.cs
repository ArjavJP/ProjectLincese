//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BussinessEntityLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class LicenseMst
    {
        public decimal License_id { get; set; }
        public string License_number { get; set; }
        public Nullable<decimal> Driver_Id { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public Nullable<System.DateTime> Expiry_date { get; set; }
        public Nullable<decimal> ApplicationId { get; set; }
    
        public virtual ApplicationMst ApplicationMst { get; set; }
        public virtual DriverMst DriverMst { get; set; }
    }
}