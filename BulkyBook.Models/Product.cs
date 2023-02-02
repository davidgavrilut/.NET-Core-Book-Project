using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models {
    public class Product {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [DisplayName("List Price (MSRP)")]
        [Range(1, 10000)]
        public double ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [Required]
        [DisplayName("Price (50)")]
        [Range(1, 10000)]
        public double Price50 { get; set; }
        [Required]
        [DisplayName("Price (100)")]
        [Range(1, 10000)]
        public double Price100 { get; set; }
        [DisplayName("Image URL")]
        public string ImageUrl { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        [DisplayName("Cover Type")]
        public int CoverTypeId { get; set; }
        [Required]
        [ForeignKey("CoverTypeId")]
        public CoverType CoverType { get; set; }
    }
}
