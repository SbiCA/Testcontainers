using System.ComponentModel.DataAnnotations;

namespace SampleApp.Api
{
    public class AddRating
    {
        [Required] 
        public string User { get; set; }

        [Required] [Range(1, 5)] 
        public int Stars { get; set; }
    }
}