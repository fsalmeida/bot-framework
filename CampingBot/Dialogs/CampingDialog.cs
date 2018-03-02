using CampingBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampingBot.Dialogs
{
    [Serializable]
    public class CampingDialog : BaseLuisDialog<object>
    {
        [NonSerialized]
        private CampingInfo campingInfo = new CampingInfo()
        {
            Address = "Rua do Camping Bonitão, 25 - CEP 02157-335 - São Paulo, SP, Brasil",
            ImagesUrl = new List<string>()
            {
                "https://gurudacidade.com.br/wp-content/uploads/2018/02/camping_tents-0.jpg",
                "https://s3.amazonaws.com/imagescloud/images/medias/camping/camping-tente.jpg",
                "http://res.muenchen-p.de/.imaging/stk/responsive/image980/dms/lhm/tourismus/camping-l/document/camping-l.jpg",
                "https://greatist.com/sites/default/files/styles/article_main/public/Campsite_featured.jpg?itok=ZZQ8wJwJ"
            },
            Name = "Terramar",
            InfrastructureItems = new List<string>() { "Fogão", "Chuveiro quente", "Banheiro", "Geladeira compartilhada", "WIFI" },
            SinglePersonTentPrice = 40,
            TwoPeopleTentPrice = 65,
            CampingAreaPrice = 30
        };

        [LuisIntent("Cumprimento")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Olá, tudo bem?\n Se precisar de informações sobre o camping {campingInfo.Name}, tais como fotos, endereço, preços ou realizar uma reserva, eu posso te ajudar");
        }

        [LuisIntent("Infra")]
        public async Task Infrastructure(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Nosso camping é equipado com:\n - " + String.Join("\n - ", campingInfo.InfrastructureItems));
        }

        [LuisIntent("Reserva")]
        public async Task Booking(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ainda to aprendendo a realizar reservas XD");
        }

        [LuisIntent("Visualizar fotos")]
        public async Task Pictures(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (var imageUrl in campingInfo.ImagesUrl)
            {
                var heroCard = new HeroCard(images: new List<CardImage>() { new CardImage(imageUrl) });
                message.Attachments.Add(heroCard.ToAttachment());
            }

            await context.PostAsync(message);
        }

        [LuisIntent("Localização")]
        public async Task Localization(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Nosso camping fica localizado em: {campingInfo.Address}");
        }

        [LuisIntent("Tarifas")]
        public async Task Rates(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Nossas tarifas são: \n - Área de camping: R$ {campingInfo.CampingAreaPrice.ToString("N2")} \n - Barraca para uma pessoa: R$ {campingInfo.SinglePersonTentPrice.ToString("N2")} \n - Barraca para duas pessoa: R$ {campingInfo.TwoPeopleTentPrice.ToString("N2")}");
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Eita, não consegui entender a frase {result.Query}");
        }
    }
}