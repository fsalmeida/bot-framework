using System;

namespace CampingAPI.Model
{
    public class BookingData
    {
        public string Id { get; set; }
        public string MainPaxName { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public bool IncludeTent { get; set; }
    }
}
