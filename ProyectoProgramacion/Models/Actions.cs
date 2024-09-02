using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProyectoProgramacion.Models
{
    public class Actions
    {
        [Key]
        public int TxNumber { get; set; }  // TX_NUMBER

        public DateTime OrderDate { get; set; }  // ORDER_DATE

        [Required]
        [StringLength(4)]
        public string Action { get; set; }  // ACTION

        [Required]
        [StringLength(10)]
        public string Status { get; set; }  // STATUS

        [Required]
        [StringLength(5)]
        public string Symbol { get; set; }  // SYMBOL

        [Required]
        public int Quantity { get; set; }  // QUANTITY

        [Required]
        [Range(0, 999.99)]
        public decimal Price { get; set; }  // PRICE

        public decimal? TotalPrice { get; set; }  // PRICE
    }
}
