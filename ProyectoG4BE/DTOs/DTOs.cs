namespace ProyectoG4BE.DTOs;

/**
 *
 * DTO significa Data Transfer Object
 * es una clase que describe SOLO los datos que el FE debe enviar
 * Id o la fecha las debe asiganr - calcular el servidor
 */

public class DTOs
{
    public string FullName { get; set; }
    
    public string Email  { get; set; } 
    
    public string Password { get; set; } 


}