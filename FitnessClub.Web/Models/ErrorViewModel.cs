namespace FitnessClub.Web.Models
{
    public class ErrorViewModel // Model voor het weergeven van foutmeldingen in de Error-view
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
