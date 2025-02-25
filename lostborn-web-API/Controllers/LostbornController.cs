﻿
using lostborn_backend.Helpers;
using lostborn_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace lostborn_backend.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentController : Controller{

    private readonly DataContext dataContext;

    public PaymentController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    //API Call to get all employees...

    [HttpGet]
    public async Task<IActionResult> GetAllMembers()
    {
        var res = await dataContext.Access.ToListAsync();
        
        return Ok(res);
    }

    //API Call to get employee by Id..(READ)
    [HttpGet("{ID}")]
    public async Task<ActionResult<IPAccess>> GetMember(int ID)
    {
        if(dataContext.Access == null)
        {
            return NotFound();
        }
        var member = await dataContext.Access.FindAsync(ID);

        if(member == null) { return NotFound(); };

        return member;
    }

    //API Call to add new entry...(CREATE)
    //we're referencing the data context object of type IPAccess class..
    [HttpPost]
    public async Task<IActionResult> AddMember([FromBody] IPAccess member)
    {
        await dataContext.Access.AddAsync(member);
        await dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(AddMember), member);
    }

    //API Call to update member using Id...(UPDATE)
    [HttpPut]
    public async Task<IActionResult>PutMember(IPAccess member)
    {
        dataContext.Entry(member).State = EntityState.Modified;

        try
        {
            await dataContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // if (!MemberExists(id)) { return NotFound(); }
            throw;
        }

        // Fetch the updated data from the database
        var updatedMember = await dataContext.Access.FindAsync(member.ID);

        // You can create a custom response object or use an anonymous object
        var response = new
        {
            Message = "Record Updated successfully",
            UpdatedData = updatedMember
        };

        // Return the custom response
        return Ok(response);
    }
    //Helper function to check if a member is present..
    private bool MemberExists(long id) { return (dataContext.Access?.Any(e => e.ID == id)).GetValueOrDefault(); }

    //API Call to delete a member using Id...(DELETE)
    [HttpDelete("{ID}")]
    public async Task<IActionResult> DeleteMember(int ID)
    {
        if(dataContext.Access == null) { return NotFound(); }

        var member = await dataContext.Access.FindAsync(ID);

        if(member == null) { return NotFound(); }

        dataContext.Access.Remove(member);

        await dataContext.SaveChangesAsync();

        // You can create a custom response object or use an anonymous object
        var response = new
        {
            Message = "Record Deleted successfully",
            UpdatedData = member
        };

        // Return the custom response
        return Ok(response);
    }
   
}
   


