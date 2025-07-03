using System.Text.Json.Serialization;

namespace GraduationProject.Data.Enums
{
    public class OrderEnums
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderStatus
        {
            NotShipped,
            Shipped,
            Delivered
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PaymentStatus
        {
            NotPaid,
            Paid
        }    
    }
}
