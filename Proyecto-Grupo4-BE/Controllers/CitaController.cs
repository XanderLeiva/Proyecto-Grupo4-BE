using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Grupo4_BE.DTOs;

namespace Proyecto_Grupo4_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class CitaController : ControllerBase
{
    private readonly CitaServices _citaServices;

    public CitaController(CitaServices citaServices)
    {
        _citaServices = citaServices;
    }

    
    //el paciente solicita una cita 
    [HttpPost]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Solicitar([FromBody] CitaDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var cita = await _citaServices.Solicitar(dto, userId);
            return Ok(cita);
        } 
        catch(Exception e)
        {
            return BadRequest(new {message = e.Message});
        }
    }
    
    //para que el paciente vea sus citas
    [HttpGet("Mis-Citas")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> GetMisCitas()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var citas = await _citaServices.ObtenerMisCitas(userId);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        }
    }
    
    
    //para que el personal vea sus citas del dia 
    [HttpGet("Agenda")]
    [Authorize(Roles = "Personal")]
    public async Task<IActionResult> GetAgenda([FromQuery] string fecha)
    {
        try
        {
            var idCentro = User.FindFirst("idCentro")?.Value;
            
            if (string.IsNullOrEmpty(idCentro))
                return Unauthorized();

            var citas = await _citaServices.ObtenerAgenda(idCentro, fecha);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        }
    }
    
    
    //cancelar una cita
    [HttpPut("{id}/cancelar")]
    [Authorize(Roles = "Paciente,Personal")]
    public async Task<IActionResult> Cancelar(string id, [FromBody] CancelarCitaDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            var idCentro = User.FindFirst("idCentro")?.Value ?? string.Empty;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var citas = await _citaServices.Cancelar(id, dto, userId, rol, idCentro);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        } 
    }
    
    
    //solo el personal puede reprogramar citas
    [HttpPut("{id}/reprogramar")]
    [Authorize(Roles = "Personal")]
    public async Task<IActionResult> Reprogramar(string id, [FromBody] ReprogramarCitaDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var idCentro = User.FindFirst("idCentro")?.Value;
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idCentro))
                return Unauthorized();

            var citas = await _citaServices.Reprogramar(id, dto, userId, idCentro);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        }
    }
    
    
    //el personal puede marcar citas como atendidas
    [HttpPut("{id}/atendida")]
    [Authorize(Roles = "Personal")]
    public async Task<IActionResult> MarcarAtendida(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var idCentro = User.FindFirst("idCentro")?.Value;
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idCentro))
                return Unauthorized();

            var citas = await _citaServices.MarcarEstado(id, EstadosCita.Atendida, userId, idCentro);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        }
    }
    
    
    //el personal puede marcar la inasistencia de un paciente a una cita
    [HttpPut("{id}/inasistencia")]
    [Authorize(Roles = "Personal")]
    public async Task<IActionResult> MarcarInasistencia(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var idCentro = User.FindFirst("idCentro")?.Value;
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idCentro))
                return Unauthorized();

            var citas = await _citaServices.MarcarEstado(id, EstadosCita.NoAsistio, userId, idCentro);
            return Ok(citas);

        }
        catch (Exception e)
        {
            return BadRequest(new {message = e.Message});
        } 
    } 
}