using AppBOs.Kernels;
using System;
using System.Collections.Generic;

namespace AppBOs.Entities;

public partial class MedicineInformation : IValidatable
{
    public string MedicineId { get; set; } = null!;

    public string MedicineName { get; set; } = null!;

    public string ActiveIngredients { get; set; } = null!;

    public string? ExpirationDate { get; set; }

    public string DosageForm { get; set; } = null!;

    public string WarningsAndPrecautions { get; set; } = null!;

    public string? ManufacturerId { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public bool Validate(out string validationError)
    {
        validationError = string.Empty;

        if (ActiveIngredients.IsValidFirstWord())
            return false;

        return true;
    }
}
