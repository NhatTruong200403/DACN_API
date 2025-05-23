﻿using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public List<IFormFile?> ImagesList { get; set; } = new List<IFormFile?>();
        public string? Description { get; set; }
        [Required]
        public int Seat { get; set; }
        public string? RentLocation { get; set; }
        [Required]
        public bool HasDriver { get; set; }
        [Required]
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        [Required]
        public bool Gear { get; set; }
        [Required]
        public string? Fuel { get; set; }
        [Required]
        public decimal FuelConsumed { get; set; }
        [Required]
        public int CarTypeId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public List<int> AmenitiesIds { get; set; } = new List<int>();
    }
}
