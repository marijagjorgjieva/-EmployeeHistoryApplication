using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeHistoryApplication.Models
{
    public class JobHistory
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public required Employee Employee { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string JobPostition { get; set; }

        [Required]
        [DateFromBeforeDateTo]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dateFrom { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dateTo { get; set; }

        public static bool IsDateRangeValid(List<JobHistory> jobHistories, DateTime newDateFrom, DateTime newDateTo)
        {
            foreach (var job in jobHistories)
            {
                if ((newDateFrom < job.dateTo && newDateTo > job.dateFrom))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class DateFromBeforeDateToAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var jobHistory = (JobHistory)validationContext.ObjectInstance;
            if (jobHistory.dateFrom >= jobHistory.dateTo)
            {
                return new ValidationResult("The start date must be before the end date.");
            }
            return ValidationResult.Success;
        }
    }
}
