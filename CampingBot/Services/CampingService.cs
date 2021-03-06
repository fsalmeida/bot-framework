﻿using CampingBot.Forms;
using CampingBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace CampingBot.Services
{
    public class CampingService
    {
        private static string _campingAPIUrl = null;
        private static string GetCampingAPIUrl()
        {
            if (_campingAPIUrl == null)
                _campingAPIUrl = ConfigurationManager.AppSettings.Get("CampingAPIUrl");

            return _campingAPIUrl;
        }

        public static async Task<CampingInfo> GetCampingInfo()
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync($"{GetCampingAPIUrl()}/camping");
            var jsonResult = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CampingInfo>(jsonResult);
        }

        public static async Task<decimal?> GetPrice(ReservationForm reservationForm)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.PostAsJsonAsync($"{GetCampingAPIUrl()}/camping/pricing", GetReservationObj(reservationForm));
            var jsonResult = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<decimal?>(jsonResult);
        }

        public static async Task<BookingResult> Book(string userId, ReservationForm reservationForm)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("UserID", userId);

            try
            {
                var response = await httpClient.PostAsJsonAsync($"{GetCampingAPIUrl()}/camping/book", GetReservationObj(reservationForm));
                var resultString = await response.Content.ReadAsStringAsync();
                var bookingResult = JsonConvert.DeserializeObject<BookingResult>(resultString);
                return bookingResult;
            }
            catch (Exception ex)
            {
                return new BookingResult() { Errors = new List<string>() { "Ocorreu um problema ao efetuar sua reserva. Por favor, tente novamente." } };
            }
        }

        public static async Task<BookingData> GetBookingData(string userId, string bookingId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("UserID", userId);

            try
            {
                var resultString = await httpClient.GetStringAsync($"{GetCampingAPIUrl()}/camping/booking/{bookingId}");
                var bookingData = JsonConvert.DeserializeObject<BookingData>(resultString);
                return bookingData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static object GetReservationObj(ReservationForm reservationForm)
        {
            return new
            {
                reservationForm.MainPaxName,
                reservationForm.NumberOfGuests,
                reservationForm.ArrivalDate,
                reservationForm.DepartureDate,
                reservationForm.IncludeTent
            };
        }
    }
}