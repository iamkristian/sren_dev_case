using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderListService.Models
{
  public class OrderList
  {
    public int Id { get; set; }
    [Required]
    public string? OrderNumber { get; set; }
    [Required]
    public string? CustomerName { get; set; }
    public DateTime? OrderDate { get; set; } = DateTime.Now;
    public ICollection<Asset>? Assets { get; set; }
  }
}
