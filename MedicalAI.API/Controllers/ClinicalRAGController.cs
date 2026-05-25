using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalAI.Core.DTOs;
using MedicalAI.Core.Interfaces;
using MedicalAI.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace MedicalAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicalRAGController : ControllerBase
    {
        private readonly IClinicalServiceWithRAG _clinicalService;
        private readonly IRAGEngine _ragEngine;

        public ClinicalRAGController(
            IClinicalServiceWithRAG clinicalService,
            IRAGEngine ragEngine)
        {
            _clinicalService = clinicalService;
            _ragEngine = ragEngine;
        }

        /// <summary>
        /// Submit checkup mới + nhận dự đoán + sinh advice từ RAG
        /// POST /api/clinical-rag/submit
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitCheckup([FromBody] AIPredictionRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var result = await _clinicalService.SubmitCheckupWithAdviceAsync(request, userId);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Checkup submitted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy kết quả dự đoán + lời khuyên từ RAG
        /// GET /api/clinical-rag/{checkupId}/result
        /// </summary>
        [HttpGet("{checkupId}/result")]
        public async Task<IActionResult> GetPredictionWithAdvice(string checkupId)
        {
            try
            {
                var result = await _clinicalService.GetPredictionWithAdviceAsync(checkupId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy advice cho một bệnh cụ thể
        /// GET /api/clinical-rag/advice?disease=Diabetes&riskScore=0.75
        /// </summary>
        [HttpGet("advice")]
        public async Task<IActionResult> GetAdviceByDisease([FromQuery] string disease, [FromQuery] double riskScore)
        {
            try
            {
                if (string.IsNullOrEmpty(disease))
                    return BadRequest("Disease name is required");

                var advice = await _ragEngine.GenerateAdviceAsync(disease, riskScore);

                return Ok(new
                {
                    success = true,
                    disease = disease,
                    riskScore = Math.Round(riskScore * 100, 2),
                    advice = advice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy mẹo phòng chống bệnh
        /// GET /api/clinical-rag/prevention?disease=Diabetes
        /// </summary>
        [HttpGet("prevention")]
        public async Task<IActionResult> GetPreventionTips([FromQuery] string disease)
        {
            try
            {
                if (string.IsNullOrEmpty(disease))
                    return BadRequest("Disease name is required");

                var prevention = await _ragEngine.GeneratePreventionAsync(disease);

                return Ok(new
                {
                    success = true,
                    disease = disease,
                    prevention = prevention
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy khuyến nghị lối sống dựa trên yếu tố nguy hiểm
        /// POST /api/clinical-rag/lifestyle-recommendations
        /// </summary>
        [HttpPost("lifestyle-recommendations")]
        public async Task<IActionResult> GetLifestyleRecommendations([FromBody] List<string> riskFactors)
        {
            try
            {
                if (riskFactors == null || !riskFactors.Any())
                    return BadRequest("Risk factors are required");

                var recommendations = await _ragEngine.GenerateLifestyleRecommendationsAsync(riskFactors);

                return Ok(new
                {
                    success = true,
                    riskFactors = riskFactors,
                    recommendations = recommendations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Sinh lời khuyên chi tiết với ngữ cảnh
        /// GET /api/clinical-rag/augmented-advice?disease=Diabetes&riskScore=0.75
        /// </summary>
        [HttpGet("augmented-advice")]
        public async Task<IActionResult> GetAugmentedAdvice([FromQuery] string disease, [FromQuery] double riskScore)
        {
            try
            {
                if (string.IsNullOrEmpty(disease))
                    return BadRequest("Disease name is required");

                var advice = await _ragEngine.AugmentAdviceWithContextAsync(disease, riskScore);

                return Ok(new
                {
                    success = true,
                    disease = disease,
                    riskScore = Math.Round(riskScore * 100, 2),
                    augmentedAdvice = advice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
