//using GraduationProject.DTO;
//using GraduationProject.Services.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace GraduationProject.Controllers
//{
//    public class AuthController:ControllerBase
//    {
//        private readonly IAuthService _authService;

//        public AuthController(IAuthService authService)
//        {
//            _authService = authService;
//        }

//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login([FromBody] PharmacyLoginDTO loginDto)
//        //{
//        //    if (loginDto == null || string.IsNullOrEmpty(loginDto.Name) || string.IsNullOrEmpty(loginDto.Password))
//        //    {
//        //        return BadRequest("Username and password are required.");
//        //    }

//        //    var token = await _authService.AuthenticateAsync(loginDto.Name, loginDto.Password);

//        //    if (token == null)
//        //    {
//        //        return Unauthorized("Invalid username or password.");
//        //    }

//        //    return Ok(new { Token = token });
//        //}
//    }
//}

