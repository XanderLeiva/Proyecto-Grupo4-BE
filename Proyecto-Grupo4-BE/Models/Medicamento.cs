namespace Proyecto_Grupo4_BE.Models;

public class Medicamento
{
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public double Dosis { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }
}