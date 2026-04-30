using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceService.Data;
using ResourcesService.Models;

namespace ResourceService.Controllers;

[ApiController]
[Route("api/resources")]
public class ResourcesController : ControllerBase
{
    private readonly ResourceDbContext _context;

    public ResourcesController(ResourceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetResources()
    {
        return Ok(await _context.Resources.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResource(int id)
    {
        var resource = await _context.Resources.FindAsync(id);

        if (resource == null)
            return NotFound();

        return Ok(resource);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResource(Resource resource)
    {
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResource(int id, Resource resource)
    {
        var existingResource = await _context.Resources.FindAsync(id);

        if (existingResource == null)
            return NotFound();

        existingResource.Name = resource.Name;
        existingResource.Type = resource.Type;
        existingResource.PricePerDay = resource.PricePerDay;
        existingResource.IsAvailable = resource.IsAvailable;

        await _context.SaveChangesAsync();

        return Ok(existingResource);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResource(int id)
    {
        var resource = await _context.Resources.FindAsync(id);

        if (resource == null)
            return NotFound();

        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Ressource supprimée avec succès",
            resource
        });
    }
}