namespace Proyecto_Grupo4_BE.DTOs;

//Dto para cancelar citas

public class CancelarCitaDto
{
    //justificacion obligatoria cuando cancela el personal de salud
    public string Justificacion { get; set; } = string.Empty;
  
}