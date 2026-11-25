namespace Proyecto_Grupo4_BE.Models;

public class Inventario
{
    public string Id { get; set; } = string.Empty;

    public string IdCentro { get; set; } = string.Empty;

    public string IdMedicamento { get; set; } = string.Empty;

    public string Lote { get; set; } = string.Empty;

    public DateTime FechaVencimiento { get; set; }

    public int Stock { get; set; }

    public int NivelMinimo { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}