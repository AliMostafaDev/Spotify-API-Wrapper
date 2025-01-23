namespace CardioTechnicalAssessment.Models
{
    public class AddToPlaylistRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Public { get; set; } = true;
    }
}
