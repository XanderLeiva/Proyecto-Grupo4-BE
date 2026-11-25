namespace Proyecto_Grupo4_BE.Models;

public class Receta
{
    public string Id { get; set; } = string.Empty;

    public string IdCita { get; set; } = string.Empty;

    public string IdPaciente { get; set; } = string.Empty;

    public string IdCentro { get; set; } = string.Empty;

    public string IdMedicamento { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public int Entregado { get; set; }

    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaRegistro { get; set; }
}
