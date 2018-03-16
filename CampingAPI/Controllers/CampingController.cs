using System.Collections.Generic;
using CampingAPI.Model;
using CampingAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CampingAPI.Controllers
{
    [Route("api/[controller]")]
    public class CampingController : Controller
    {
        [HttpGet]
        public CampingInfo CampingInfo()
        {
            CampingInfo campingInfo = new CampingInfo()
            {
                Address = "Rua do Camping Bonitão, 25 - CEP 02157-335 - São Paulo, SP, Brasil",
                ImagesUrl = new List<string>()
                {
                    "https://upload.wikimedia.org/wikipedia/commons/0/01/CampingNearTheBeach.jpeg",
                    "https://macamp.com.br/guia/wp-content/uploads//arquivos/guia/arquivos/88/imagens/17a6d085b71.jpg",
                    "https://i3.wp.com/trekkingbrasil.com/wp-content/uploads/lona-sobre-barraca.jpg",
                    "http://www.webcamping.com.br/campings/pedaserra07.jpg",
                    "http://www.portalcanoaquebrada.com.br/camping_canoa_quebrada/camping_canoa_quebrada_03.jpg"
                },
                Name = "Terramar",
                InfrastructureItems = new List<string>() { "Fogão", "Chuveiro quente", "Banheiro", "Geladeira compartilhada", "WIFI" },
                SinglePersonTentPrice = Model.BookingData.SinglePersonTentPrice,
                TwoPeopleTentPrice = Model.BookingData.TwoPeopleTentPrice,
                CampingAreaPrice = Model.BookingData.CampingAreaPrice
            };
            return campingInfo;
        }

        [HttpPost]
        [Route("pricing")]
        public decimal? Pricing([FromBody]BookingData bookingData)
        {
            if (!bookingData.IsValid())
                return null;

            bookingData.SetPrice();
            return bookingData.Price;
        }

        [HttpPost]
        [Route("book")]
        public BookingResult Book([FromBody]BookingData bookingData)
        {
            if (!bookingData.IsValid())
                return new BookingResult() { Errors = new List<string>() { "Informações inválidas para a reserva" } };

            StringValues values;
            if (Request.Headers.TryGetValue("UserID", out values))
            {
                bookingData.SetPrice();
                var userId = values[0];
                BookingRepository.AddBooking(userId, bookingData);

                return new BookingResult()
                {
                    Id = bookingData.Id
                };
            }
            else
                return new BookingResult() { Errors = new List<string>() { "Usuário não identificado" } };
        }

        [HttpGet]
        [Route("booking/{bookingId}")]
        public BookingData BookingData([FromRoute] string bookingId)
        {
            StringValues values;
            if (Request.Headers.TryGetValue("UserID", out values))
            {
                var userId = values[0];
                return BookingRepository.GetBooking(userId, bookingId);
            }
            else
                return null;
        }
    }
}
