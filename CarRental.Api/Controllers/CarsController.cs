using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Api.Data;
using CarRental.Api.Models;
using CarRental.Api.DTOs;

namespace CarRental.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/cars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarDTO>>> GetCars()
    {
        var cars = await _context.Cars.ToListAsync();

        return Ok(cars.Select(ToDTO));
    }

    // GET: api/cars/1
    [HttpGet("{id}")]
    public async Task<ActionResult<CarDTO>> GetCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);

        if (car == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(car));
    }

    // POST: api/cars
    [HttpPost]
    public async Task<ActionResult<CarDTO>> CreateCar(CarDTO carDTO)
    {
        if (!IsValidYear(carDTO.Year))
        {
            return BadRequest(
                "El automóvil debe tener un año válido y no superar los 5 años de antigüedad."
            );
        }

        var car = ToEntity(carDTO);

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        carDTO.Id = car.Id;

        return CreatedAtAction(
            nameof(GetCar),
            new { id = car.Id },
            carDTO
        );
    }

    // PUT: api/cars/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(
        int id, CarDTO carDTO)
    {
        if (id != carDTO.Id)
        {
            return BadRequest("Los ID no coinciden.");
        }

        if (!IsValidYear(carDTO.Year))
        {
            return BadRequest(
                "El automóvil debe tener un año válido y no superar los 5 años de antigüedad."
            );
        }

        var car = await _context.Cars.FindAsync(id);

        if (car == null)
        {
            return NotFound();
        }

        car.Brand = carDTO.Brand;
        car.Model = carDTO.Model;
        car.Price = carDTO.Price;
        car.Year = carDTO.Year;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/cars/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);

        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Validar el año del automóvil
    private bool IsValidYear(int year)
    {
        int currentYear = DateTime.Now.Year;

        return year >= currentYear - 5
            && year <= currentYear;
    }

    // Convertir DTO a entidad
    private Car ToEntity(CarDTO dto)
    {
        return new Car
        {
            Brand = dto.Brand,
            Model = dto.Model,
            Price = dto.Price,
            Year = dto.Year
        };
    }

    // Convertir entidad a DTO
    private CarDTO ToDTO(Car car)
    {
        return new CarDTO
        {
            Id = car.Id,
            Brand = car.Brand,
            Model = car.Model,
            Price = car.Price,
            Year = car.Year
        };
    }
}