using fundooNotes.DTOs.Notes;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;
using fundooNotes.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace fundooNotes.Services.Implementations
{
    public class NotesService : INotesService
    {
        private readonly IDistributedCache _cache;
        private readonly INotesRepository _notesRepository;

        public NotesService(INotesRepository notesRepository, IDistributedCache cache)
        {
            _notesRepository = notesRepository;
            _cache = cache;
        }

        public async Task<NoteResponseDTO> CreateNote(CreateNoteDTO dto, int userId)
        {
            Note note = new Note()
            {
                Title = dto.Title,
                Description = dto.Description,
                Reminder = dto.Reminder,
                UserId = userId,
                CreatedDate = DateTime.Now,
                LastModifiedAt = DateTime.Now
            };

            _notesRepository.CreateNote(note);

            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            return new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            };
        }

        // implement caching
        public async Task<List<NoteResponseDTO>> GetAllNotes(int userId)
        {
            string cacheKey = $"notes_{userId}";

            // checking if data exists in redis then return
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<NoteResponseDTO>>(cachedData);
            }

            var notes = _notesRepository.GetAllNotes(userId);
            var notesDto = notes.Select(note => new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            }).ToList();

            // storing data to cache
            var cacheOptions = new DistributedCacheEntryOptions()
                               .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));  

            var serializedNotes = JsonSerializer.Serialize(notesDto);
            await _cache.SetStringAsync(cacheKey, serializedNotes, cacheOptions);
            return notesDto;
        }

        public async Task<NoteResponseDTO> GetNoteById(int noteId, int userId)
        {
            string cacheKey = $"notes_{userId}_{noteId}";
            // checking if note present in cache
            var cachedNote = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedNote))
            {
                return JsonSerializer.Deserialize<NoteResponseDTO>(cachedNote);
            }

            // retrieve from db and store in cache
            var note = _notesRepository.GetNoteById(noteId, userId);
            if (note == null)
                return null;
            var noteDto = new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            };


            // storing data to cache
            var cacheOptions = new DistributedCacheEntryOptions()
                               .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            var serializedNote = JsonSerializer.Serialize(noteDto);
            await _cache.SetStringAsync(cacheKey, serializedNote, cacheOptions);

            return noteDto;
        }

        public async Task<NoteResponseDTO> UpdateNote(int noteId, UpdateNoteDTO dto, int userId)
        {
            var note = _notesRepository.GetNoteById(noteId, userId);

            if (note == null)
                return null;

            note.Title = dto.Title;
            note.Description = dto.Description;
            note.LastModifiedAt = DateTime.Now;

            _notesRepository.UpdateNote(note, userId);
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            };
        }


        public async Task<bool> ToggleArchive(int noteId, int userId)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);
            return _notesRepository.ToggleArchive(noteId, userId);
        }

        public async Task<bool> TogglePin(int noteId, int userId)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return _notesRepository.TogglePin(noteId, userId);
        }

        public async Task<bool> ChangeColor(int noteId, int userId, string color)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return _notesRepository.ChangeColor(noteId, userId, color);
        }

        public async Task<bool> SetReminder(int noteId, int userId, DateTime? reminder)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return _notesRepository.SetReminder(noteId, userId, reminder);
        }

        public List<NoteResponseDTO> GetTrashNotes(int userId)
        {
            var notes = _notesRepository.GetTrashNotes(userId);

            return notes.Select(note => new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            }).ToList();
        }
        public List<NoteResponseDTO> GetArchiveNotes(int userId)
        {
            var notes = _notesRepository.GetArchiveNotes(userId);

            return notes.Select(note => new NoteResponseDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Description = note.Description,
                IsPin = note.IsPin,
                IsArchived = note.IsArchived,
                IsTrash = note.IsTrash,
                Colour = note.Colour,
                Reminder = note.Reminder,
                CreatedDate = note.CreatedDate
            }).ToList();
        }

        public async Task<bool> DeleteNote(int noteId, int userId)
        {
            var note = _notesRepository.GetNoteById(noteId, userId);

            if (note == null)
                return false;

            note.IsArchived = false;
            note.IsPin = false;
            note.IsTrash = true;
            note.LastModifiedAt = DateTime.Now;

            _notesRepository.UpdateNote(note, userId);

            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return true;
        }
        public async Task<bool> RecoverNote(int noteId, int userId)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return _notesRepository.RecoverNote(noteId, userId);
        }

        public async Task<bool> DeleteNotePermanently(int noteId, int userId)
        {
            string cacheKey = $"notes_{userId}";
            await _cache.RemoveAsync(cacheKey);

            string cacheKey2 = $"notes_{userId}_{noteId}";
            await _cache.RemoveAsync(cacheKey2);

            return _notesRepository.DeleteNotePermanently(noteId, userId);
        }
    }
}

// clearing the cache when -> create,update,delete(move to trash),recover