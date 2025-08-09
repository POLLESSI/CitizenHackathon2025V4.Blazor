namespace CitizenHackathon2025V4.Blazor.Client.Shared.Suggestion
{
    public class SuggestionDTO
    {
#nullable disable
        public string SuggestedAlternatives { get; set; }
        public string Reason { get; set; }
        public DateTime DateSuggestion { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Category { get; set; } = default!;
        public string Source { get; set; } = default!;
    }
}
