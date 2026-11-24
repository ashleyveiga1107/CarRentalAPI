using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Api.Data;
using CarRental.Api.Models;
using CarRental.Api.DTOs;

namespace CarRental.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RentalsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRental(RentalDTO dto)
    {
        // Validar fechas
        if (dto.StartDate == default ||
            dto.EndDate == default ||
            dto.StartDate >= dto.EndDate)
        {
            return BadRequest("Las fechas no son válidas.");
        }

        // Comprobar que el automóvil existe
        var car = await _context.Cars.FindAsync(dto.CarId);

        if (car == null)
        {
            return NotFound("El automóvil no existe.");
        }

        // Comprobar que el usuario existe
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null)
        {
            return NotFound("El usuario no existe.");
        }

        // Comprobar disponibilidad
        bool isRented = await _context.Rentals.AnyAsync(r =>
            r.CarId == dto.CarId &&
            r.StartDate < dto.EndDate &&
            r.EndDate > dto.StartDate
        );

        if (isRented)
        {
            return Conflict(
                "El automóvil ya está alquilado en esas fechas."
            );
        }

        // Calcular cantidad de días
        int totalDays = (dto.EndDate - dto.StartDate).Days;

        if (totalDays <= 0)
        {
            return BadRequest(
                "El alquiler debe durar al menos un día."
            );
        }

        // Calcular precio total
        decimal totalPrice = totalDays * car.Price;

        // Crear el alquiler
        var rental = new Rental
        {
            UserId = dto.UserId,
            CarId = dto.CarId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Price = totalPrice
        };

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            RentalId = rental.Id,
            Price = rental.Price
        });
    }
}