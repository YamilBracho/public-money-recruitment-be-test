using System;

namespace VacationRental.Api.Models
{
    public class BookingBindingModel
    {
        public int RentalId { get; set; }

        public DateTime Start
        {
            get => _startIgnoreTime;
            set => _startIgnoreTime = value.Date;
        }

        private DateTime _startIgnoreTime;
        public int Nights { get; set; }

        // One Booking always occupies only one unit
        public int Unit { get; set; }

        public int PreparationTime { get; set; }

        public DateTime EndCleaningDate
        {
            get => Start.AddDays(PreparationTime);
        }

       /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nights"></param>
        /// <param name="rentalId"></param>
        /// <param name="start"></param>
        /// <param name="unit"></param>
        /// <param name="preparationTime"></param>
        public BookingBindingModel(int nights, int rentalId, DateTime start, int unit, int preparationTime)
        {
            RentalId = rentalId;
            Start = start.Date;
            Nights = nights;
            Unit = unit;
            PreparationTime = preparationTime;
        }


    }
}
