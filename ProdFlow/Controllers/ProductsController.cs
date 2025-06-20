﻿using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using ProdFlow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context;

        public ProductsController(IProductService productService, AppDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        [HttpGet("GetOptions")]
        public async Task<IActionResult> GetOptions()
        {
            try
            {
                var options = await _productService.GetOptionsAsync();
                return Ok(options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProductOptionResponse
                {
                    Lignes = new List<string>(),
                    Famille = new List<string>(),
                    SousFamilles = new List<string>(),
                    Types = new List<string>(),
                    Statuts = new List<string>()
                });
            }
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(
            [FromForm(Name = "Ligne")] string ligne,
            [FromForm(Name = "Famille")] string famille,
            [FromForm(Name = "Sous-Famille")] string sousFamille,
            [FromForm(Name = "Code Produit")] string codeProduit,
            [FromForm(Name = "Libellé")] string libelle,
            [FromForm(Name = "Serialisé")] bool isSerialized,
            [FromForm(Name = "Type")] string type = null,
            [FromForm(Name = "Libellé 2")] string libelle2 = null,
            [FromForm(Name = "Statut")] string statut = null,
            [FromForm(Name = "Code Client (C264)")] string codeProduitClientC264 = null,
            [FromForm(Name = "Poids (kg)")] double? poids = null, // Changed from float? to double?
            [FromForm(Name = "Créé par")] string createur = null,
            [FromForm(Name = "Date Création")] DateTime? dateCreation = null,
            [FromForm(Name = "Tolerance")] string tolerance = null,
            [FromForm(Name = "Flashable")] byte? flashable = null,
            [FromForm(Name = "GalliaName")] string galliaName = null,
            [FromForm(Name = "Verification Deadline")] DateTime? verificationDeadline = null) // Added
        {
            int? galliaId = null;
            if (!string.IsNullOrWhiteSpace(galliaName))
            {
                var gallia = await _context.Gallias
                    .FirstOrDefaultAsync(g => g.LabelName == "Gallia" && g.GalliaName == galliaName.Trim());
                if (gallia == null)
                {
                    return BadRequest(new
                    {
                        Result = "Error",
                        Message = $"GalliaName '{galliaName}' with LabelName 'Gallia' not found",
                        ProductCode = codeProduit?.Trim()
                    });
                }
                galliaId = gallia.GalliaId;
            }

            var dto = new ProduitSerialiséDto
            {
                LpNum = ligne?.Trim(),
                FpCod = famille?.Trim(),
                SpCod = sousFamille?.Trim(),
                PtNum = codeProduit?.Trim(),
                PtLib = libelle?.Trim(),
                IsSerialized = isSerialized,
                TpCod = type?.Trim(),
                PtLib2 = libelle2?.Trim(),
                SpId = statut?.Trim(),
                PtSpecifT14 = codeProduitClientC264?.Trim(),
                PtPoids = poids,
                PtCreateur = createur?.Trim(),
                PtDcreat = dateCreation,
                PtVerificationDeadline = verificationDeadline, // Added
                PtSpecifT15 = tolerance?.Trim(),
                PtFlasher = flashable,
                GalliaId = galliaId
            };

            var result = await _productService.CreateProductAsync(dto);

            return result.Result == "Success"
                ? Ok(new
                {
                    Result = result.Result,
                    Message = result.Message,
                    ProductCode = result.ProductCode,
                    IsSerialized = isSerialized
                })
                : BadRequest(new
                {
                    Result = result.Result,
                    Message = result.Message,
                    ProductCode = result.ProductCode,
                    IsSerialized = isSerialized
                });
        }

        [HttpDelete("DeleteProduct/{ptNum}")]
        public async Task<IActionResult> DeleteProduct(string ptNum)
        {
            var result = await _productService.DeleteProductAsync(ptNum);
            return result.Result == "Success" ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(
             [FromQuery][Required(ErrorMessage = "IsApproved is required")] bool isApproved,
             [FromQuery] string CodeProduit = null,
             [FromQuery] string status = null,
             [FromQuery] bool? isSerialized = null)
        {
            var result = await _productService.GetProductAsync(isApproved, CodeProduit, status, isSerialized);
            return result.Result == "Success"
                ? Ok(new
                {
                    Result = result.Result,
                    Message = result.Message,
                    Products = result.Products
                })
                : BadRequest(new
                {
                    Result = result.Result,
                    Message = result.Message,
                    Products = result.Products
                });
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(
            [FromForm(Name = "Code Produit")] string codeProduit,
            [FromForm(Name = "Libellé")] string libelle,
            [FromForm(Name = "Serialisé")] bool isSerialized,
            [FromForm(Name = "Ligne")] string ligne = null,
            [FromForm(Name = "Famille")] string famille = null,
            [FromForm(Name = "Sous-Famille")] string sousFamille = null,
            [FromForm(Name = "Type")] string type = null,
            [FromForm(Name = "Libellé 2")] string libelle2 = null,
            [FromForm(Name = "Statut")] string statut = null,
            [FromForm(Name = "Code Client (C264)")] string codeProduitClientC264 = null,
            [FromForm(Name = "Poids (kg)")] double? poids = null, // Changed from float? to double?
            [FromForm(Name = "Créé par")] string createur = null,
            [FromForm(Name = "Date Création")] DateTime? dateCreation = null,
            [FromForm(Name = "Tolerance")] string tolerance = null,
            [FromForm(Name = "Flashable")] byte? flashable = null,
            [FromForm(Name = "GalliaName")] string galliaName = null,
            [FromForm(Name = "Verification Deadline")] DateTime? verificationDeadline = null) // Added
        {
            int? galliaId = null;
            if (!string.IsNullOrWhiteSpace(galliaName))
            {
                var gallia = await _context.Gallias
                    .FirstOrDefaultAsync(g => g.LabelName == "Gallia" && g.GalliaName == galliaName.Trim());
                if (gallia == null)
                {
                    return BadRequest(new
                    {
                        Result = "Error",
                        Message = $"GalliaName '{galliaName}' with LabelName 'Gallia' not found",
                        ProductCode = codeProduit?.Trim()
                    });
                }
                galliaId = gallia.GalliaId;
            }

            var dto = new ProduitSerialiséDto
            {
                PtNum = codeProduit?.Trim(),
                PtLib = libelle?.Trim(),
                IsSerialized = isSerialized,
                LpNum = ligne?.Trim(),
                FpCod = famille?.Trim(),
                SpCod = sousFamille?.Trim(),
                TpCod = type?.Trim(),
                PtLib2 = libelle2?.Trim(),
                SpId = statut?.Trim(),
                PtSpecifT14 = codeProduitClientC264?.Trim(),
                PtPoids = poids,
                PtCreateur = createur?.Trim(),
                PtDcreat = dateCreation,
                PtVerificationDeadline = verificationDeadline, // Added
                PtSpecifT15 = tolerance?.Trim(),
                PtFlasher = flashable,
                GalliaId = galliaId
            };

            var result = await _productService.UpdateProductAsync(dto);
            return result.Result == "Success" ? Ok(result) : BadRequest(result);
        }

        [HttpPost("VerifyProduct")]
        public async Task<IActionResult> VerifyProduct([FromBody] VerifyProductDto dto)
        {
            var result = await _productService.VerifyProductAsync(dto, User.Identity?.Name ?? "Anonymous");
            return result.Result == "Success" ? Ok(result) : BadRequest(result);
        }
        [HttpGet("test-sendgrid")]
        public async Task<string> TestSendGrid()
        {
            return await _productService.TestSendGridEmailAsync();
        }
    }
}