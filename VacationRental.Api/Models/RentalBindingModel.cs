namespace VacationRental.Api.Models
{
    public class RentalBindingModel
    {
        public int Units { get; set; }

        public RentalBindingModel(int units)
        {
            this.Units = units;
        }
    }
}
