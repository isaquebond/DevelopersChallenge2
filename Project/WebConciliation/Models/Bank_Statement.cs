using System.ComponentModel.DataAnnotations;

namespace WebConciliation.Models
{
    public class Bank_Statement
    {
        [Display(Name = "Date")]
        public string Date { get; set; }

        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

    }
}