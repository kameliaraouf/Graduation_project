using GraduationProject.Data.DTO;
using GraduationProject.Data.Enums;
using System;
using System.Collections.Generic;

namespace GraduationProject.Data.DTO
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderEnums.OrderStatus Status { get; set; }
        public OrderEnums.PaymentStatus PaymentStatus { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Governorate { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReceiptIMG { get; set; }
        public int PharmacyID { get; set; }
        public string PharmacyName { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public PaymentOrderDTO Payment { get; set; }
        public string ConfirmationCode { get; set; }
    }
}





