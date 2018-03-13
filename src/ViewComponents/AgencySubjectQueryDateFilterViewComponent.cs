using System;
using System.Threading.Tasks;
using Clinician.ApiClients.AgencyClient;
using Clinician.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clinician.ViewComponents
{
    public class AgencySubjectQueryDateFilter : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(AgencySubject agencySubject)
        {
            var model = new AgencySubjectQueryDateFilterViewModel(agencySubject);
            var today = DateTime.Today;
            
            model.Filters.Add("Today", (today, new DateTime(today.Year, today.Month, today.Day, 23, 59, 59)));
            model.Filters.Add("This month", (new DateTime(today.Year, today.Month, 1), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)));
            model.Filters.Add("Last 3 months", (new DateTime(today.Year, today.Month, 1).AddMonths(-3), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)));
            model.Filters.Add("Last 6 months", (new DateTime(today.Year, today.Month, 1).AddMonths(-6), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)));

            return await Task.FromResult(this.View(model));
        }
    }
}
