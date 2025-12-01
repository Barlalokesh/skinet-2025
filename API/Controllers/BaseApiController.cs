using System;
using Microsoft.AspNetCore.Mvc;
using core.Interface;
using core.Entities;
using API.RequestHelpers;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repo,
      ISpecification<T> spec, int PageIndex, int pageSize ) where T : BaseEntity
    {
       var items = await repo.ListAsync(spec);
        
         var count = await repo.CountAsync(spec);
    
          var pagination = new  Pagination<T>(PageIndex, pageSize, count, items);
 
          return Ok(pagination);

    } 
    

}
