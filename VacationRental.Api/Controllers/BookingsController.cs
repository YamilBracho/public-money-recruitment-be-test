using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            if (!_bookings.ContainsKey(bookingId))
                throw new ApplicationException($"Booking not found: {bookingId}");

            return _bookings[bookingId];
        }

        private void CheckIfFull(BookingBindingModel model, int units, ref DateTime start, ref DateTime end, ref bool isFull)
        {
            var startDate = model.Start;
            // The lastdate for this booking is the number of night plus
            // the number of daus for cleaning
            var endDate = startDate.Add(TimeSpan.FromDays(model.Nights + model.PreparationTime));

           int count = 0;
           foreach (var booking in _bookings.Values) 
           {
               if (booking.RentalId != model.RentalId || booking.Unit != model.Unit)
               {
                    continue;
               }

               if (booking.Start >= startDate && booking.Start < endDate)
               {
                  count++;
               }
           }

           // Any booking fits the condition, so the unit for this rentak is available
           if (count == 0) 
           {
                start = startDate; 
                end = endDate;
                isFull = false;
           }
           else 
           {
                start = startDate;
                end = endDate;
                isFull = count >= units;
           }
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");

            RentalViewModel rentalViewModel = null;
            if (!_rentals.TryGetValue(model.RentalId, out rentalViewModel))
            {
                 throw new ApplicationException("Rental not found:{model.RentalId}");
            }

            // Check If the unit for this rental is available
             DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            bool isFull = false;
            CheckIfFull(model, rentalViewModel.Units, ref start, ref end, ref isFull);
            if (isFull)
            {
                throw new ApplicationException($"Rental {model.RentalId} is unavailable because it is full from {start.ToShortDateString()} To {end.ToShortDateString()}.");
            }

            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };
            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Nights = model.Nights,
                Unit = model.Unit,
                PreparationTimeInDays = model.PreparationTime
            });

            return key;
        }
    }
}
