namespace Proyecto_Grupo4_BE.Models;

public class Cita
{
    public string Id { get; set; } = string.Empty;
    
    public string IdPaciente { get; set; } = string.Empty;
    
    public string IdCentro  { get; set; } = string.Empty;
    
    public string Servicio { get; set; } = string.Empty;
    
    public string FechaCita { get; set; } = string.Empty;
    
    public string EstadoCita { get; set; } = EstadosCita.Programada;
    
    public string MotivoEstado { get; set; } = string.Empty;
    
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
    
    public string ActualizadaPor { get; set; } = string.Empty;
}