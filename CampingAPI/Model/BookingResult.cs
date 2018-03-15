using System.Collections.Generic;

namespace CampingAPI.Model
{
    public class BookingResult
    {
        public string Id { get; set; }
        public List<string> Errors { get; set; }
    }
}
