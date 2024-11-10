using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Common.Models.Models;


public partial class Customer
{
    [Key]
    [Column("customerid")]
    public int CustomerId { get; set; }

    [Required]
    [Column("firstname")]
    [StringLength(255)]
    public string? FirstName { get; set; }

    [Required]
    [Column("lastname")]
    [StringLength(255)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Phone]
    [Column("phonenumber")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    
    [Column("address")]
    public string? Address { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
} 

