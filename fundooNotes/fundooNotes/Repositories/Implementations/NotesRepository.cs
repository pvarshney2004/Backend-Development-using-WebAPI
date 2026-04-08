using fundooNotes.Repositories.Interfaces;
using fundooNotes.Context;
using fundooNotes.Models;
using Microsoft.EntityFrameworkCore;

namespace fundooNotes.Repositories.Implementations
{
    public class NotesRepository : INotesRepository
    {
        private readonly FundooContext _context;
        // automatically injects DbContext here(DI)
        public NotesRepository(FundooContext context)
        {
            _context = context;
        }

        // create new note
        public Note CreateNote(Note note)
        {
            _context.Notes.Add(note);
            _context.SaveChanges();
            return note;
        }

        // get all notes for a user
        public List<Note> GetAllNotes(int userId)
        {
            //return _context.Notes.Where(n => n.UserId == userId && !n.IsTrash).ToList();

            // for collaborator to see all notes
            // this condition check whether user created note or user is a collaborator
            var notes = _context.Notes.Where(n =>
            (n.UserId == userId ||
            _context.Collaborators.Any(c => c.UserId == userId))
            && !n.IsTrash && !n.IsArchived)
        .ToList();
            //var notes = _context.Notes.Where(n => n.UserId == userId && !n.IsTrash).ToList();

            return notes;
        }

       

        // update existing note
        public Note UpdateNote(Note note)
        {
            _context.Notes.Update(note);
            _context.SaveChanges();
            return note;
        }

        public Note UpdateNote(Note note, int userId)
        {
            var existingNote = _context.Notes.FirstOrDefault(n =>
                n.NoteId == note.NoteId &&
                (n.UserId == userId ||
                 _context.Collaborators.Any(c => c.NoteId == n.NoteId && c.UserId == userId)));

            if (existingNote == null)
                return null;

            existingNote.Title = note.Title;
            existingNote.Description = note.Description;

            _context.SaveChanges();

            return existingNote;
        }

        // remove note from database
        public void DeleteNote(Note note)
        {
            _context.Notes.Remove(note);
            _context.SaveChanges();
        }

        // toggle archive status of a note
        public bool ToggleArchive(int noteId, int userId)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.NoteId == noteId && n.UserId == userId);
            if (note == null)
                return false;
            note.IsArchived = !note.IsArchived;
            _context.SaveChanges();
            return true;
        }

        // toggle pin status of a note
        public bool TogglePin(int noteId, int userId)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.NoteId == noteId && n.UserId == userId);
            if (note == null)
                return false;
            note.IsPin = !note.IsPin;
            _context.SaveChanges();
            return true;
        }

        public bool ChangeColor(int noteId, int userId, string color)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.NoteId == noteId && n.UserId == userId);
            if (note == null)
                return false;
            note.Colour = color;
            _context.SaveChanges();
            return true;
        }

        public bool SetReminder(int noteId, int userId, DateTime? reminder)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.NoteId == noteId && n.UserId == userId);
            if (note == null)
                return false;
            note.Reminder = reminder;
            _context.SaveChanges();
            return true;
        }

        public List<Note> GetTrashNotes(int userId)
        {
            return _context.Notes.Where(n => (n.UserId == userId && n.IsTrash)).ToList();
        }

        public List<Note> GetArchiveNotes(int userId)
        {
            return _context.Notes.Where(n => (n.UserId == userId && n.IsArchived)).ToList();
        }

        public bool RecoverNote(int noteId, int userId)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.NoteId == noteId && n.UserId == userId && n.IsTrash);

            if (note == null)
                return false;

            note.IsTrash = false;

            _context.SaveChanges();

            return true;
        }

        public bool DeleteNotePermanently(int noteId, int userId)
        {
            var note = _context.Notes
                .FirstOrDefault(n => (n.NoteId == noteId && n.UserId == userId && n.IsTrash));
            if (note == null)
                return false;
            _context.Notes.Remove(note);
            _context.SaveChanges();
            return true;
        }




        public Note GetNoteById(int noteId, int userId)
        {
            var note = _context.Notes.FirstOrDefault(n => n.NoteId==noteId && n.UserId==userId);

            return note;
        }
    }
}
