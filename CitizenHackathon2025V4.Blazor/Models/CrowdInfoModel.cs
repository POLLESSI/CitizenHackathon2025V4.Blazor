namespace CitizenHackathon2025V4.Blazor.Client.Models
{
    public class CrowdInfoModel
    {
    #nullable disable
        public int Id { get; set; }
        public string LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CrowdLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
