using EmployeeHistoryApplication.Models;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;


namespace EmployeeHistoryApplication.Models
{
    public class JobHistory
    {
        public int Id { get; set; } 
        public required int EmployeeId { get; set; }
        public required Employee Employee { get; set; } = null!;
        public required string CompanyName { get; set; }
        public required string JobPostition { get; set; }
        public required DateTime dateFrom { get; set; }

        public DateTime? dateTo { get; set; }

        public static bool IsDateRangeValid(List<JobHistory> jobHistories, DateTime newDateFrom, DateTime? newDateTo)
        {
            
            foreach (var job in jobHistories)
            {
                if ((job.dateTo!=null&&newDateFrom < job.dateTo && newDateTo > job.dateFrom))
                {
                    return false;
                }
                else if ((job.dateTo == null && newDateFrom < DateTime.Now && newDateTo > job.dateFrom))
                {
                    return false;
                }
            }
            return true;
        }

     
    }
}


public class JobHistoryDateComparerDescending : IComparer<JobHistory>
{
    public int Compare(JobHistory x, JobHistory y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        return y.dateFrom.CompareTo(x.dateFrom); // Descending order
    }
}


