﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProjectLicenseEntities : DbContext
    {
        public ProjectLicenseEntities()
            : base("name=ProjectLicenseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ApplicationMst> ApplicationMsts { get; set; }
        public virtual DbSet<DriverMst> DriverMsts { get; set; }
        public virtual DbSet<LicenseMst> LicenseMsts { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserMst> UserMsts { get; set; }
    }
}
