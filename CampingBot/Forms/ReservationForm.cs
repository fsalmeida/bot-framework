using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace CampingBot.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Desculpe, não entendi")]
    public class ReservationForm
    {
        const string MainPaxNameDescription = "Nome do passageiro principal";
        const string NumberOfGuestsDescription = "Número total de hóspedes";
        const string ArrivalDateDescription = "Data da chegada";
        const string DepartureDateDescription = "Data da volta";
        const string IncludeTentDescription = "Incluir tenda";

        [Describe(MainPaxNameDescription)]
        public string MainPaxName { get; set; }

        [Describe(NumberOfGuestsDescription)]
        public int NumberOfGuests { get; set; }

        [Describe(ArrivalDateDescription)]
        public DateTime ArrivalDate { get; set; }

        [Describe(DepartureDateDescription)]
        public DateTime DepartureDate { get; set; }

        [Describe(IncludeTentDescription)]
        public bool IncludeTent { get; set; }

        public static IForm<ReservationForm> BuildForm()
        {
            var form = new FormBuilder<ReservationForm>();
            form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;
            form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yep" };
            form.Configuration.No = new string[] { "não", "nao", "no", "not", "n" };
            form.Message("Preciso de algumas informações para gerar sua reserva");
            form.Field(nameof(MainPaxName), MainPaxNameDescription);
            form.Field(nameof(NumberOfGuests), NumberOfGuestsDescription, validate: ValidateNumberOfGuests);
            form.Field(nameof(ArrivalDate), ArrivalDateDescription, validate: ValidateArrivalDate);
            form.Field(nameof(DepartureDate), DepartureDateDescription, validate: ValidateDepartureDate);
            form.AddRemainingFields();
            form.Confirm(new PromptAttribute("Pode confirmar as informações?\n{*}\n\nSim \\ Não?"));
            return form.Build();
        }

        static ValidateAsyncDelegate<ReservationForm> ValidateNumberOfGuests = async (state, value) =>
        {
            long totalNumberOfGuests = (long)value;
            if (totalNumberOfGuests <= 0)
                return new ValidateResult() { IsValid = false, Feedback = "Uma reserva deve ter ao menos um hóspede" };
            else if (totalNumberOfGuests > 6)
                return new ValidateResult() { IsValid = false, Feedback = "Uma reserva não pode ter mais do que 6 hóspedes" };

            return new ValidateResult() { IsValid = true, Value = totalNumberOfGuests };
        };

        static ValidateAsyncDelegate<ReservationForm> ValidateArrivalDate = async (state, value) =>
        {
            DateTime arrivalDate = (DateTime)value;
            if (arrivalDate <= DateTime.Today)
                return new ValidateResult() { IsValid = false, Feedback = "Data deve ser maior do que hoje" };

            return new ValidateResult() { IsValid = true, Value = arrivalDate };
        };

        static ValidateAsyncDelegate<ReservationForm> ValidateDepartureDate = async (state, value) =>
        {
            DateTime departureDate = (DateTime)value;
            if (departureDate <= state.ArrivalDate)
                return new ValidateResult() { IsValid = false, Feedback = "Data da volta deve ser posterior a data da chegada" };

            return new ValidateResult() { IsValid = true, Value = departureDate };
        };
    }
}