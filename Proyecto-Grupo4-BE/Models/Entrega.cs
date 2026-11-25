namespace Proyecto_Grupo4_BE.Models;

public class Entrega
{
    public string Id { get; set; } = string.Empty;

    public string IdReceta { get; set; } = string.Empty;

    public string IdInventario { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public DateTime FechaRegistro { get; set; }
}