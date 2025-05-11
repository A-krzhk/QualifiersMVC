using DanceualifiersMVC.Data;
using DanceualifiersMVC.Models;
using DanceualifiersMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Services;

public class DirectionService
{
    private readonly ApplicationDbContext _context;
    
    public DirectionService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<DirectionViewModel>> GetAllDirectionsAsync()
    {
        var directions = await _context.Directions
            .Include(d => d.TimeSlots)
            .ThenInclude(ts => ts.Registrations)
            .Select(d => new DirectionViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                TimeSlots = d.TimeSlots.Select(ts => new TimeSlotViewModel
                {
                    Id = ts.Id,
                    DirectionId = ts.DirectionId,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    MaxParticipants = ts.MaxParticipants,
                    AvailableSeats = ts.MaxParticipants - ts.Registrations.Count
                }).ToList()
            })
            .ToListAsync();
            
        return directions;
    }
    
    public async Task<DirectionViewModel> GetDirectionByIdAsync(int id)
    {
        var direction = await _context.Directions
            .Include(d => d.TimeSlots)
            .ThenInclude(ts => ts.Registrations)
            .FirstOrDefaultAsync(d => d.Id == id);
            
        if (direction == null)
            throw new KeyNotFoundException("Направление не найдено");
            
        return new DirectionViewModel
        {
            Id = direction.Id,
            Name = direction.Name,
            Description = direction.Description,
            TimeSlots = direction.TimeSlots.Select(ts => new TimeSlotViewModel
            {
                Id = ts.Id,
                DirectionId = ts.DirectionId,
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                MaxParticipants = ts.MaxParticipants,
                AvailableSeats = ts.MaxParticipants - ts.Registrations.Count
            }).ToList()
        };
    }
    
    public async Task<DirectionViewModel> CreateDirectionAsync(DirectionViewModel model, string userId)
    {
        // Проверка существования пользователя
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException($"Пользователь с ID {userId} не существует.");
        }

        var direction = new Direction
        {
            Name = model.Name,
            Description = model.Description,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Directions.Add(direction);
        await _context.SaveChangesAsync();

        return new DirectionViewModel
        {
            Id = direction.Id,
            Name = direction.Name,
            Description = direction.Description,
            TimeSlots = new List<TimeSlotViewModel>()
        };
    }
    
    public async Task<TimeSlotViewModel> AddTimeSlotAsync(int directionId, TimeSlotViewModel model)
    {
        var direction = await _context.Directions.FindAsync(directionId);
        if (direction == null)
            throw new KeyNotFoundException("Направление не найдено");
            
        var timeSlot = new TimeSlot
        {
            DirectionId = directionId,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            MaxParticipants = model.MaxParticipants
        };
        
        _context.TimeSlots.Add(timeSlot);
        await _context.SaveChangesAsync();
        
        return new TimeSlotViewModel
        {
            Id = timeSlot.Id,
            DirectionId = timeSlot.DirectionId,
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime,
            MaxParticipants = timeSlot.MaxParticipants,
            AvailableSeats = timeSlot.MaxParticipants
        };
    }
    
    public async Task DeleteDirectionAsync(int directionId)
    {
        var direction = await _context.Directions
            .FirstOrDefaultAsync(d => d.Id == directionId);
    
        if (direction == null)
            throw new KeyNotFoundException("Направление не найдено");

        _context.Directions.Remove(direction);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteTimeSlotAsync(int timeSlotId)
    {
        var timeSlot = await _context.TimeSlots
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);
            
        if (timeSlot == null)
            throw new KeyNotFoundException("Временной интервал не найден");
            
        _context.TimeSlots.Remove(timeSlot);
        await _context.SaveChangesAsync();
    }
    
    public async Task<DirectionViewModel> UpdateDirectionAsync(DirectionViewModel model)
    {
        var direction = await _context.Directions
            .FirstOrDefaultAsync(d => d.Id == model.Id);

        if (direction == null)
            throw new KeyNotFoundException("Направление не найдено");

        direction.Name = model.Name;
        direction.Description = model.Description;

        _context.Directions.Update(direction);
        await _context.SaveChangesAsync();

        return new DirectionViewModel
        {
            Id = direction.Id,
            Name = direction.Name,
            Description = direction.Description,
            TimeSlots = await _context.TimeSlots
                .Where(ts => ts.DirectionId == direction.Id)
                .Select(ts => new TimeSlotViewModel
                {
                    Id = ts.Id,
                    DirectionId = ts.DirectionId,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    MaxParticipants = ts.MaxParticipants,
                    AvailableSeats = ts.MaxParticipants - ts.Registrations.Count
                }).ToListAsync()
        };
    }
}