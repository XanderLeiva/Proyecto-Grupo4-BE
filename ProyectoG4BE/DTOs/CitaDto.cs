namespace ProyectoG4BE.DTOs;

//DTO para crear citas
public class CitaDto
{
  public string IdCentro { get; set; } = string.Empty;
  
  public string Servicio { get; set; } = string.Empty;  
  
  public string Fecha { get; set; } = string.Empty;
}