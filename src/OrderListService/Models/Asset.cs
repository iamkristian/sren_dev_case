using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderListService.Models
{
  public class Asset
  {
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? FileFormat { get; set; }
    public string? FileSize { get; set; }
    public string? Path { get; set; }
    public int OrderListId { get; set; }
    public OrderList? OrderList { get; set; }
  }
}
