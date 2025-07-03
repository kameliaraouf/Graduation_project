using GraduationProject.Data;
using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using GraduationProject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   [Authorize]
    public class DiagnosisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DiagnosisController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ApplicationDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _environment = environment;
            _context = context;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<DiagnosisDTO>> Predict([FromForm] CreateDiagnosisDTO dto)
        {
            try
            {
                // Check for authenticated user
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User is not authenticated or token is missing." });
                }
                var userId = int.Parse(userIdClaim.Value);

                if (dto == null)
                    return BadRequest(new { message = "Request body is null." });

                if (dto.Image == null || dto.Image.Length == 0)
                    return BadRequest(new { message = "Image is required and must not be empty." });

                // Save uploaded file
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                // Call model server
                var client = _httpClientFactory.CreateClient();
                using var content = new MultipartFormDataContent();
                using var imageStream = System.IO.File.OpenRead(filePath);
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(dto.Image.ContentType);
                content.Add(imageContent, "file", dto.Image.FileName);

                var modelServerUrl = _configuration["ModelServer:Url"];
                var response = await client.PostAsync(modelServerUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Model server error", details = responseContent });
                }

                var jsonObject = JObject.Parse(responseContent);
                var predictions = jsonObject["predictions"]?.ToObject<Dictionary<string, Dictionary<string, object>>>();

                if (predictions == null)
                {
                    return BadRequest(new { message = "Model response format is invalid or missing predictions." });
                }

                var skinDiseases = new[]
                {
            "Acne", "Actinic", "Atopic", "Basal", "Benign", "Bites", "Bullous", "Cancer",
            "Candidiasis", "Carcinoma", "Dermatitis", "Eczema", "Eruptions", "Exanthems", "Fungal",
            "Fungus", "HPV", "Herpes", "Infections", "Infestations", "Keratoses", "Keratosis",
            "Lesions", "Lichen", "Lyme", "Malignant", "Melanoma", "Moles", "Molluscum", "Nail",
            "Nevi", "Pigmentation", "Planus", "Psoriasis", "Ringworm", "Rosacea", "Scabies",
            "Seborrheic", "STDs", "Systemic", "Tinea", "Tumors", "Vascular", "Viral", "Warts"
        };

                var top5Predictions = predictions
                    .Where(p => p.Value.ContainsKey("confidence"))
                    .OrderByDescending(p => Convert.ToDecimal(p.Value["confidence"]))
                    .Take(5)
                    .ToList();

                var matched = top5Predictions
                    .Where(p => skinDiseases.Contains(p.Key))
                    .OrderByDescending(p => Convert.ToDecimal(p.Value["confidence"]))
                    .FirstOrDefault();

                var diagnosis = new Diagnosis
                {
                    Result = "Unable to identify the condition or it is not related to skin diseases.",
                    Accuracy = 0,
                    Date = DateTime.Now,
                    img = imageUrl,
                    UserID = userId
                };

                if (!string.IsNullOrEmpty(matched.Key))
                {
                    decimal confidence = Convert.ToDecimal(matched.Value["confidence"]);
                    if (confidence >= 0.64M)
                    {
                        diagnosis.Result = matched.Key;
                        diagnosis.Accuracy = confidence * 100;
                    }
                }

                _context.Diagnoses.Add(diagnosis);
                await _context.SaveChangesAsync();

                return Ok(new DiagnosisDTO
                {
                    Result = diagnosis.Result,
                    Accuracy = diagnosis.Accuracy,
                    img = diagnosis.img
                });
            }
            catch (Exception ex)
            {
                // This will expose the real problem
                return StatusCode(500, new
                {
                    message = "Unhandled server error.",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiagnosisDTO>>> GetAll()
        {
            var diagnoses = await _context.Diagnoses
                .Select(d => new DiagnosisDTO
                {
                    Result = d.Result,
                    Accuracy = d.Accuracy,
                    img = d.img
                })
                .ToListAsync();

            return Ok(diagnoses);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<DiagnosisDTO>>> GetForUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var diagnoses = await _context.Diagnoses
                .Where(d => d.UserID == userId)
                .OrderByDescending(d => d.Date)
                .Select(d => new DiagnosisDTO
                {
                    Result = d.Result,
                    Accuracy = d.Accuracy,
                    img = d.img
                })
                .ToListAsync();

            return Ok(diagnoses);
        }


    }
}




