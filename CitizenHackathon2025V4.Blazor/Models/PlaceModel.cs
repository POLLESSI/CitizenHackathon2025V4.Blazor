namespace CitizenHackathon2025V4.Blazor.Client.Models
{
    public class PlaceModel
    {
    #nullable disable
        public int Id { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }
        public bool Indoor { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Capacity { get; set; }
        public string Tag { get; set; }
    }
}
