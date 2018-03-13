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

        private async Task ReservationFormCompletedAsync(IDialogContext context, IAwaitable<ReservationForm> result)
        {
            var reservationForm = await result;
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