using EF.Concurrency.Data;
using EF.Concurrency.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF.Concurrency.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(DataContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        try
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.First();
            var entity = entry.Entity as Product;
            var databaseValues = await entry.GetDatabaseValuesAsync();
            var databaseEntity = databaseValues?.ToObject() as Product;
            var clientValues = entry.CurrentValues;

            if (databaseValues is null)
            {
                return BadRequest("This product removed by another user.");
            }
            else
            {
                return BadRequest("This product updated by another user.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
