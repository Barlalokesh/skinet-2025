using System;
using Microsoft.AspNetCore.Mvc;
using core.Entities;

namespace API.Errors;

public class ApiErrorResponse(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
    public string? Details { get; set; } = details;
}

