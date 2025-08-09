namespace CitizenHackathon2025V4.Blazor.Client.Models
{
    public class EventModel
    {
    #nullable disable
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateEvent { get; set; }
        public int ExpectedCrowd { get; set; }
        public bool IsOutdoor { get; set; }
    }
}
