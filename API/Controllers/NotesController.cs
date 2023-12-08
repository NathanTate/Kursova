using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

[Authorize]
public class NotesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public NotesController(IUserRepository userRepository, DataContext context, IMapper mapper)
    {
        _userRepository = userRepository;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> CreateNote(NotesDto notesDto)
    {
        var userId = User.GetUserId();
        if(await NoteExists(notesDto.Title, userId)) return BadRequest("Note with such a title already exists");

        var note = _mapper.Map<Note>(notesDto);
        note.AppUserId = userId;

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{noteId}")]
    public async Task<ActionResult> DeleteNote(int noteId)
    {
        var note = await _context.Notes.Where(n => n.AppUserId == User.GetUserId()).FirstOrDefaultAsync(n => n.Id == noteId);
        if(note == null) return NotFound();

        _context.Notes.Remove(note);
        if(await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting note");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateNote(UpdateNoteDto updateNoteDto)
    {
        var notes = _context.Notes.AsQueryable();
        notes = notes.Where(note => note.AppUserId == User.GetUserId());

        if(await notes.AnyAsync(n => n.Title == updateNoteDto.Title.ToLower() && n.Id != updateNoteDto.Id)) return BadRequest("Change the title buddy");

        var note = await notes.FirstOrDefaultAsync(n => n.Id == updateNoteDto.Id);
        if(note == null) return NotFound();
        
        _mapper.Map(updateNoteDto, note);
        note.Title = updateNoteDto.Title.ToLower();

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem updating note");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
    {
        var query = _context.Notes.AsQueryable();
        query = query.Where(note => note.AppUserId == User.GetUserId());
        var notes = await query.ToListAsync();

        return Ok(notes);
    }

    [HttpGet("{noteId}")]
    public async Task<ActionResult<Note>> GetNote(int noteId)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.AppUserId == User.GetUserId() && n.Id == noteId);
        if(note == null) return NotFound();
        
        return Ok(note);
    }


    private async Task<bool> NoteExists(string title, int userId)
    {
        return await _context.Notes.Where(n => n.AppUserId == userId).AnyAsync(n => n.Title == title.ToLower());
    }
}
