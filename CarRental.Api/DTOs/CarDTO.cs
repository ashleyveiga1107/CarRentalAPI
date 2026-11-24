using System.ComponentModel.DataAnnotations;

namespace CarRental.Api.DTOs;

public class CarDTO
{
    public int Id { get; set; }

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Year { get; set; }
}