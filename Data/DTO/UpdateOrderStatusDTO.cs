using System.Text.Json.Serialization;
using GraduationProject.Data.Entities;
using System;
using System.Collections.Generic;
using GraduationProject.Data.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace GraduationProject.Data.DTO
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public OrderEnums.OrderStatus Status { get; set; }

        [Required]
        public OrderEnums.PaymentStatus PaymentStatus { get; set; }

        [JsonIgnore]
        public IFormFile? ReceiptImage { get; set; }

    }
}
