using System;

namespace CampingAPI.Model
{
    public class BookingData
    {
        public const decimal CampingAreaPrice = 30;
        public const decimal SinglePersonTentPrice = 40;
        public const decimal TwoPeopleTentPrice = 65;

        public string Id { get; set; }
        public string MainPaxName { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public bool IncludeTent { get; set; }
        public decimal Price { get; set; }

        public bool IsValid()
        {
            var numberOfNights = this.DepartureDate.Subtract(this.ArrivalDate).Days;
            return this.NumberOfGuests > 0 && numberOfNights > 0;
        }

        internal void SetPrice()
        {
            var numberOfNights = this.DepartureDate.Subtract(this.ArrivalDate).Days;
            if (this.NumberOfGuests <= 0 || numberOfNights <= 0)
                return;

            var totalPrice = this.NumberOfGuests * CampingAreaPrice;
            if (this.IncludeTent)
            {
                var tentPrice = CalculateTentPrice(this.NumberOfGuests);
                totalPrice += tentPrice;
            }

            this.Price = totalPrice * numberOfNights;
        }

        private static decimal CalculateTentPrice(int numberOfGuests)
        {
            var numberOfTwoPeopleTent = Convert.ToInt32(numberOfGuests / 2);
            var tentPrice = numberOfTwoPeopleTent * TwoPeopleTentPrice;

            if (numberOfGuests % 2 > 0)
                tentPrice += SinglePersonTentPrice;

            return tentPrice;
        }
    }
}
