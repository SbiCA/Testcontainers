using System.Collections.Generic;

namespace SampleApp
{
    public class Review
    {
        public string Movie { get; set; }

        public List<Rating> Ratings { get; set; }
    }
}