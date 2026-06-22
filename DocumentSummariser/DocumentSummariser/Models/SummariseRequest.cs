using System.ComponentModel.DataAnnotations;

namespace DocumentSummariser.Models
{
    public class SummariseRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
