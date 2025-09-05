namespace Inventory.Core.Dtos;

public class RequisitionDto
{
    public int IdRequisitions { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime InsertDate { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public List<MaterialRequisitionDto> Materials { get; set; } = new List<MaterialRequisitionDto>();
}

public class MaterialRequisitionDto
{
    public int IdMaterialRequisition { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialShortName { get; set; } = string.Empty;
    public int MeasurementUnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string UnitShortName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class RequisitionSummaryDto
{
    public int IdRequisitions { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public DateTime InsertDate { get; set; }
    public string? Username { get; set; }
    public int MaterialCount { get; set; }
    public decimal TotalQuantity { get; set; }
}