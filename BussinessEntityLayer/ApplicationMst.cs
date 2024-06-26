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
    
    public partial class ApplicationMst
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApplicationMst()
        {
            this.LicenseMsts = new HashSet<LicenseMst>();
        }
    
        public decimal ApplicationId { get; set; }
        public string LicenseNumber { get; set; }
        public Nullable<decimal> Driver_Id { get; set; }
        public Nullable<System.DateTime> Creation_date { get; set; }
        public Nullable<System.DateTime> Expiry_date { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Userid { get; set; }
        public Nullable<bool> IsRenew { get; set; }
    
        public virtual DriverMst DriverMst { get; set; }
        public virtual UserMst UserMst { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LicenseMst> LicenseMsts { get; set; }
    }
}
