namespace RentalHosting_64130107.Models
{
    public class RegisterContractViewModel
    {
        public int HostingId { get; set; }
        public int Months { get; set; }
        public IEnumerable<HostingModel_64130107> Hostings { get; set; } = new List<HostingModel_64130107>();
    }
}