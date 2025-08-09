namespace CitizenHackathon2025V4.Blazor.Client.Models
{
    public class TrafficConditionModel
    {
    #nullable disable
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateCondition { get; set; }
        public string IncidentType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }
}
