using CampingBot.Forms;
using CampingBot.Model;
using CampingBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CampingBot.Dialogs
{
    [Serializable]
    public class CampingDialog : BaseLuisDialog<object>
    {
        [NonSerialized]
        private static CampingInfo _campingInfo = null;
        private CampingInfo GetCampingInfo()
        {
            if (_campingInfo == null)
            {
                var campingTask = Task.Run(() => CampingService.GetCampingInfo());
                campingTask.Wait();
                _campingInfo = campingTask.Result;
            }

            return _campingInfo;
        }

        [LuisIntent("Cumprimento")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Olá, tudo bem?\n Se precisar de informações sobre o camping {GetCampingInfo().Name}, tais como fotos, endereço, preços ou realizar uma reserva, eu posso te ajudar");
        }

        [LuisIntent("Infra")]
        public async Task Infrastructure(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Nosso camping é equipado com:\n - " + String.Join("\n - ", GetCampingInfo().InfrastructureItems));
        }

        [LuisIntent("Reserva")]
        public async Task Booking(IDialogContext context, LuisResult result)
        {
            FormDialog<ReservationForm> form = new FormDialog<ReservationForm>(new ReservationForm(), ReservationForm.BuildForm, FormOptions.PromptInStart, cultureInfo: new CultureInfo("pt-BR"));
            context.Call(form, ReservationFormCompletedAsync);
        }

        [LuisIntent("InformacoesReserva")]
        public async Task BookingInfo(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.FirstOrDefault();
            if (entity != null)
            {
                var bookingId = entity.Entity;
                var bookingData = await CampingService.GetBookingData(context.Activity.From.Id, bookingId);
                if (bookingData != null)
                    await context.PostAsync($"\n\nID: {bookingData.Id}\n\nPassageiro principal: {bookingData.MainPaxName}\n\nNúmero total de hóspedes: {bookingData.NumberOfGuests}" +
                        $"\n\nData da chegada: {bookingData.ArrivalDate.ToString("dd/MM/yyyy")}\n\nData da volta: {bookingData.DepartureDate.ToString("dd/MM/yyyy")}" +
                        $"\n\nIncluir barraca: {(bookingData.IncludeTent ? "Sim" : "Não")}\n\nValor da reserva: R$ {bookingData.Price.ToString("N2")}");
                else
                    await context.PostAsync("Não consegui identificar o número da sua reserva.");
            }
            else
                await context.PostAsync("Não consegui identificar o número da sua reserva.");
        }

        private async Task ReservationFormCompletedAsync(IDialogContext context, IAwaitable<ReservationForm> result)
        {
            var reservationForm = await result;
            context.UserData.SetValue("reservationForm", reservationForm);
            decimal? price = await CampingService.GetPrice(reservationForm);

            if (price == null)
            {
                await context.PostAsync("Existem dados inválidos na sua reserva, por favor tente novamente.");
                context.Wait(MessageReceived);
            }
            else
                PromptDialog.Confirm(context, AfterReservationConfirmationAsync, $"Sua reserva totalizou R$ {price.Value.ToString("N2")}. Posso confirmar?", "Não entendi, por favor, responda 'Sim' ou 'Não'", promptStyle: PromptStyle.Keyboard);
        }

        private async Task AfterReservationConfirmationAsync(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                var confirm = await result;
                if (confirm)
                {
                    ReservationForm reservationForm = context.UserData.GetValue<ReservationForm>("reservationForm");

                    var bookingResult = await CampingService.Book(context.Activity.From.Id, reservationForm);

                    if (bookingResult.Errors != null && bookingResult.Errors.Count > 0)
                        await context.PostAsync("Houve um erro ao emitir sua reserva. Por favor, tente novamente.");
                    else
                        await context.PostAsync($"Sua reserva foi confirmada com o ID {bookingResult.Id}.");
                }
                else
                    await context.PostAsync("Ok, posso te ajudar em mais alguma coisa?");
            }
            finally
            {
                context.UserData.RemoveValue("reservationForm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Visualizar fotos")]
        public async Task Pictures(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (var imageUrl in GetCampingInfo().ImagesUrl)
            {
                var heroCard = new HeroCard(images: new List<CardImage>() { new CardImage(imageUrl) });
                message.Attachments.Add(heroCard.ToAttachment());
            }

            await context.PostAsync(message);
        }

        [LuisIntent("Localização")]
        public async Task Localization(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Nosso camping fica localizado em: {GetCampingInfo().Address}");
        }

        [LuisIntent("Tarifas")]
        public async Task Rates(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Nossas tarifas são: \n - Área de camping: R$ {GetCampingInfo().CampingAreaPrice.ToString("N2")} \n - Barraca para uma pessoa: R$ {GetCampingInfo().SinglePersonTentPrice.ToString("N2")} \n - Barraca para duas pessoa: R$ {GetCampingInfo().TwoPeopleTentPrice.ToString("N2")}");
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Eita, não consegui entender a frase {result.Query}");
        }
    }
}