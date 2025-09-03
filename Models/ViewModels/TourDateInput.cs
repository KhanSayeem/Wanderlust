using System;
using System.ComponentModel.DataAnnotations;

namespace TourismApp.Models.ViewModels
{
    public class TourDateInput
    {
        public int? Id { get; set; }              // existing row id (Edit only)

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Range(1, 1000)]
        public int? Capacity { get; set; }

        public bool Remove { get; set; }          // Edit: tick to delete this date
    }
}
