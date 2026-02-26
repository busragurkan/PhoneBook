using Contact.API.DTOs;
using Contact.API.Services;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.Shared.DTOs;

namespace Contact.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> GetAll()
    {
        var contacts = await _contactService.GetAllContactsAsync();
        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactDetailDto>> GetById(Guid id)
    {
        var contact = await _contactService.GetContactDetailAsync(id);
        if (contact == null)
            return NotFound();

        return Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
    {
        var created = await _contactService.CreateContactAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _contactService.DeleteContactAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{contactId:guid}/contact-informations")]
    public async Task<ActionResult<ContactInformationDto>> AddContactInformation(
        Guid contactId,
        [FromBody] CreateContactInformationDto dto)
    {
        var created = await _contactService.AddContactInformationAsync(contactId, dto);
        if (created == null)
            return NotFound("Contact not found.");

        return Created($"/api/contacts/{contactId}/contact-informations", created);
    }

    [HttpDelete("contact-informations/{id:guid}")]
    public async Task<IActionResult> RemoveContactInformation(Guid id)
    {
        var result = await _contactService.RemoveContactInformationAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<LocationStatisticsDto>> GetLocationStatistics([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location parameter is required.");

        var stats = await _contactService.GetLocationStatisticsAsync(location);
        return Ok(stats);
    }
}
