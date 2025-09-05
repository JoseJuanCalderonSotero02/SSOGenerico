using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security;
using Module = Inventory.Core.Entities.Module; 
using Permission = Inventory.Core.Entities.Permission; 
using RolePermission = Inventory.Core.Entities.RolePermission; 

namespace Inventory.Infrastructure.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    // SSO Tables
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Module> Modules { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    // Inventory Tables
    public DbSet<MaterialCategory> MaterialCategories { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialsMeasurementUnits> MaterialsMeasurementUnits { get; set; }
    public DbSet<MaterialSupplier> MaterialSuppliers { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<MaterialRequisition> MaterialRequisitions { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<MaterialPurchaseOrder> MaterialPurchaseOrders { get; set; }
    public DbSet<InvoicePurchaseOrder> InvoicePurchaseOrders { get; set; }
    public DbSet<TypeMovementInventory> TypeMovementInventories { get; set; }
    public DbSet<MovementInventory> MovementInventories { get; set; }
    public DbSet<InventoryByBranch> InventoryByBranches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar esquemas y nombres de tabla EXACTOS
        modelBuilder.Entity<User>().ToTable("Users", "sso");
        modelBuilder.Entity<Role>().ToTable("Roles", "sso");
        modelBuilder.Entity<UserRole>().ToTable("UserRoles", "sso");

        // Inventory Tables - NOMBRES EXACTOS
        modelBuilder.Entity<MaterialCategory>().ToTable("MaterialCategory", "inv");
        modelBuilder.Entity<MeasurementUnit>().ToTable("MeasurementUnits", "inv");
        modelBuilder.Entity<Supplier>().ToTable("Suppliers", "inv");
        modelBuilder.Entity<Branch>().ToTable("Branches", "inv");
        modelBuilder.Entity<Material>().ToTable("Materials", "inv");
        modelBuilder.Entity<MaterialsMeasurementUnits>().ToTable("MaterialsMeasurementUnits", "inv");
        modelBuilder.Entity<MaterialSupplier>().ToTable("MaterialsSupplier", "inv");
        modelBuilder.Entity<Status>().ToTable("Statuses", "inv");
        modelBuilder.Entity<Requisition>().ToTable("Requisitions", "inv");
        modelBuilder.Entity<MaterialRequisition>().ToTable("MaterialRequisition", "inv");
        modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrders", "inv");
        modelBuilder.Entity<MaterialPurchaseOrder>().ToTable("MaterialsPurchaseOrders", "inv");
        modelBuilder.Entity<InvoicePurchaseOrder>().ToTable("InvoicePurchaseOrders", "inv");
        modelBuilder.Entity<TypeMovementInventory>().ToTable("TypeMovementsInventory", "inv");
        modelBuilder.Entity<MovementInventory>().ToTable("MovementsInventory", "inv");
        modelBuilder.Entity<InventoryByBranch>().ToTable("InventoryByBranch", "inv");

        // Configuración de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasColumnType("varbinary(30)").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2(0)");
        });

        // Configuración de Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRoles);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2(0)");
        });

        // Configuración de UserRole
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.IdUsers, e.IdRoles });
            entity.Property(e => e.AssignedDate).HasColumnType("datetime2(0)");

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.IdUsers)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.IdRoles)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.AssignedBy)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedByIdUsers)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Modules", "sso");
            entity.HasKey(e => e.IdModule);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Route).HasMaxLength(200);
            entity.Property(e => e.Icon).HasMaxLength(50);

            // Self-referencing relationship for parent/child modules
            entity.HasOne(m => m.ParentModule)
                .WithMany(m => m.ChildModules)
                .HasForeignKey(m => m.ParentModuleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Permission
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions", "sso");
            entity.HasKey(e => e.IdPermission);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(p => p.Module)
                .WithMany(m => m.Permissions)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de RolePermission
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions", "sso");
            entity.HasKey(e => e.IdRolePermission);

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de MaterialCategory
        modelBuilder.Entity<MaterialCategory>(entity =>
        {
            entity.HasKey(e => e.IdMaterialCategory);
            entity.Property(e => e.Name).HasMaxLength(30).IsRequired();
        });

        // Configuración de MeasurementUnit
        modelBuilder.Entity<MeasurementUnit>(entity =>
        {
            entity.HasKey(e => e.IdMeasurementUnits);
            entity.Property(e => e.Name).HasMaxLength(30).IsRequired();
            entity.Property(e => e.ShortName).HasMaxLength(10).IsRequired();
        });

        // Configuración de Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.IdSuppliers);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TaxNumber).HasMaxLength(150).IsRequired();
            entity.Property(e => e.TaxName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.NameContact).HasMaxLength(100);
        });

        // Configuración de Branch
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.IdBranches);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.IdCompany).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(200).IsRequired();
        });

        // Configuración de Material
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.IdMaterials);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ShortName).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(m => m.MaterialCategory)
                .WithMany()
                .HasForeignKey(m => m.MaterialCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de MaterialsMeasurementUnits
        modelBuilder.Entity<MaterialsMeasurementUnits>(entity =>
        {
            entity.HasKey(e => e.IdMaterialsMeasurementUnits);
            entity.HasIndex(e => new { e.MaterialId, e.MeasurementUnitId }).IsUnique();

            entity.HasOne(mmu => mmu.Material)
                .WithMany()
                .HasForeignKey(mmu => mmu.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mmu => mmu.MeasurementUnit)
                .WithMany()
                .HasForeignKey(mmu => mmu.MeasurementUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mmu => mmu.User)
                .WithMany()
                .HasForeignKey(mmu => mmu.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de MaterialSupplier
        modelBuilder.Entity<MaterialSupplier>(entity =>
        {
            entity.HasKey(e => e.IdMaterialsSupplier);
            entity.HasIndex(e => new { e.MaterialId, e.SupplierId }).IsUnique();
            entity.Property(e => e.Cost).HasColumnType("decimal(19,4)");

            entity.HasOne(ms => ms.Material)
                .WithMany()
                .HasForeignKey(ms => ms.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ms => ms.Supplier)
                .WithMany()
                .HasForeignKey(ms => ms.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ms => ms.User)
                .WithMany()
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Status
        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.IdStatuses);
            entity.HasIndex(e => new { e.Entity, e.Name }).IsUnique();
            entity.Property(e => e.Entity).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        // Configuración de Requisition
        modelBuilder.Entity<Requisition>(entity =>
        {
            entity.HasKey(e => e.IdRequisitions);
            entity.Property(e => e.InsertDate).HasColumnType("datetime2(0)");

            entity.HasOne(r => r.Branch)
                .WithMany()
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Status)
                .WithMany()
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de MaterialRequisition
        modelBuilder.Entity<MaterialRequisition>(entity =>
        {
            entity.HasKey(e => e.IdMaterialRequisition);
            entity.HasIndex(e => new { e.RequisitionId, e.MaterialMeasurementUnitId }).IsUnique();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");

            entity.HasOne(mr => mr.Requisition)
                .WithMany(r => r.MaterialRequisitions)
                .HasForeignKey(mr => mr.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mr => mr.MaterialMeasurementUnit)
                .WithMany()
                .HasForeignKey(mr => mr.MaterialMeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de PurchaseOrder
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.IdPurchaseOrders);
            entity.Property(e => e.InsertDate).HasColumnType("datetime2(0)");

            entity.HasOne(po => po.Supplier)
                .WithMany()
                .HasForeignKey(po => po.SuppliersId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(po => po.Requisition)
                .WithMany()
                .HasForeignKey(po => po.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(po => po.Branch)
                .WithMany()
                .HasForeignKey(po => po.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(po => po.Status)
                .WithMany()
                .HasForeignKey(po => po.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(po => po.User)
                .WithMany()
                .HasForeignKey(po => po.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de MaterialPurchaseOrder
        modelBuilder.Entity<MaterialPurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.IdMaterialsPurchaseOrders);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.QuantityIn).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Cost).HasColumnType("decimal(19,4)");

            entity.HasOne(mpo => mpo.PurchaseOrder)
                .WithMany(po => po.MaterialPurchaseOrders)
                .HasForeignKey(mpo => mpo.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mpo => mpo.MaterialSupplier)
                .WithMany()
                .HasForeignKey(mpo => mpo.MaterialSupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mpo => mpo.MaterialMeasurementUnit)
                .WithMany()
                .HasForeignKey(mpo => mpo.MaterialMeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de InvoicePurchaseOrder
        modelBuilder.Entity<InvoicePurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.IdInvoicePurchaseOrders);
            entity.Property(e => e.UUID).HasMaxLength(36).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(19,4)");
            entity.Property(e => e.InsertDate).HasColumnType("datetime2(0)");

            entity.HasOne(ipo => ipo.PurchaseOrder)
                .WithMany()
                .HasForeignKey(ipo => ipo.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de TypeMovementInventory
        modelBuilder.Entity<TypeMovementInventory>(entity =>
        {
            entity.HasKey(e => e.IdTypeMovementsInventory);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        // Configuración de MovementInventory
        modelBuilder.Entity<MovementInventory>(entity =>
        {
            entity.HasKey(e => e.IdMovementsInventory);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,4)");
            entity.Property(e => e.InsertDate).HasColumnType("datetime2(0)");

            entity.HasOne(mi => mi.TypeMovementInventory)
                .WithMany()
                .HasForeignKey(mi => mi.TypeMovementsInventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mi => mi.Branch)
                .WithMany()
                .HasForeignKey(mi => mi.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mi => mi.MaterialSupplier)
                .WithMany()
                .HasForeignKey(mi => mi.MaterialSupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mi => mi.MaterialMeasurementUnit)
                .WithMany()
                .HasForeignKey(mi => mi.MaterialMeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mi => mi.User)
                .WithMany()
                .HasForeignKey(mi => mi.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de InventoryByBranch
        modelBuilder.Entity<InventoryByBranch>(entity =>
        {
            entity.HasKey(e => e.IdInventoryByBranch);
            entity.HasIndex(e => new { e.BranchId, e.MaterialSupplierId, e.MaterialMeasurementUnitId }).IsUnique();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Cost).HasColumnType("decimal(18,4)");

            entity.HasOne(ib => ib.Branch)
                .WithMany()
                .HasForeignKey(ib => ib.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ib => ib.MaterialSupplier)
                .WithMany()
                .HasForeignKey(ib => ib.MaterialSupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ib => ib.MaterialMeasurementUnit)
                .WithMany()
                .HasForeignKey(ib => ib.MaterialMeasurementUnitId)
                .OnDelete(DeleteBehavior.Cascade);
        }


        );

        base.OnModelCreating(modelBuilder);
    }
}