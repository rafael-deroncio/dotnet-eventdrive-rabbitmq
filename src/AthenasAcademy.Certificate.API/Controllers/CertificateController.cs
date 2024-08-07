﻿using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AthenasAcademy.Certificate.API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class CertificateController(ICertificateService certificateService) : ControllerBase
{
    private readonly ICertificateService _certificateService = certificateService;

    [HttpGet("{register}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CertificateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCertificate(string register)
        => Ok(await _certificateService.GetCertificate(register));

    [HttpPost("generate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CertificateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] CertificateRequest request)
         => Ok(await _certificateService.CreateCertificate(request));
}
