﻿using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class SalePromotionDTO : BaseModelDTO
    {
        public required string Content { get; set; }
        public decimal DiscountValue { get; set; }
        public required DateTime ExpiredDate { get; set; }
        [JsonPropertyOrder(99)]
        public int PromotionTypeId { get; set; }
        [JsonPropertyOrder(100)]
        public string? PromotionTypeName { get; set; }
    }
}
