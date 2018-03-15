using CampingAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static BookingData GetBooking(string userId, string bookingId)
        {
            BookingData bookingData = null;

            if (bookings.ContainsKey(userId))
                bookingData = bookings[userId].FirstOrDefault(booking => booking.Id == bookingId);

            return bookingData;
        }

        private static void GenerateId(BookingData bookingData)
        {
            bookingData.Id = Guid.NewGuid().ToString().Split('-')[0];
        }
    }
}
