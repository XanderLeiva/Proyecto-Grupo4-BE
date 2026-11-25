using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Grupo4_BE.DTOs;
using Proyecto_Grupo4_BE.Services;


namespace Proyecto_Grupo4_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService) { 
        _authService = authService;
    }
    
    // POST /api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var user = await _authService.Register(dto);
            return Ok(new { user.Id, user.FullName, user.Email });
        }
        catch (Exception ex)
        {
            return BadRequest(new  {message = ex.Message});
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.Login(dto);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new  {message = ex.Message});
        }
    }
    
}