using CampingAPI.Model;
using System;
using System.Collections.Generic;

namespace CampingAPI.Repositories
{
    public class BookingRepository
    {
        private static Dictionary<string, List<BookingData>> bookings = new Dictionary<string, List<BookingData>>();

        public static void AddBooking(string userId, BookingData bookingData)
        {
            if (!bookings.ContainsKey(userId))
                bookings.Add(userId, new List<BookingData>());

            GenerateId(bookingData);
            List<BookingData> userReservations = bookings[userId];
            userReservations.Add(bookingData);
        }

        private static void GenerateId(BookingData bookingData)
        {
            bookingData.Id = Guid.NewGuid().ToString().Split('-')[0];
        }
    }
}
