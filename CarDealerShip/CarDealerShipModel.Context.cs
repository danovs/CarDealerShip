﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarDealerShip
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CarDealerShipEntities : DbContext
    {
        public CarDealerShipEntities()
            : base("name=CarDealerShipEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CustomerReview> CustomerReviews { get; set; }
        public virtual DbSet<Deal> Deals { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
