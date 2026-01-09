using System;
using Microsoft.AspNetCore.Mvc;
using core.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("unauthorized")]
   public ActionResult GetUnAuthorized()
   {
      return Unauthorized();
   }

    [HttpGet("badrequest")]
public ActionResult GetBadRequest()
{
    return BadRequest(" Not a good request ");
}

    [HttpGet("Notfound")]
   public ActionResult GetNotFound()
   {
      return NotFound();
   }

    [HttpGet("Internalerror")]
   public ActionResult GetInternalError()
   {
      throw new Exception("This is a test exception");
   }

    [HttpPost("validationerror")]
   public ActionResult GetValidationError(CreateProductDto product)
   {
      return Ok();
}

[Authorize]
[HttpGet("secret")]
public IActionResult GetSecret()
{
   var name = User.FindFirst(ClaimTypes.Name)?.Value;
   var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok("Hello " + name + " with the id of " + id);
}
}