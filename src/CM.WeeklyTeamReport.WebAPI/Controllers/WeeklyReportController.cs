﻿using CM.WeeklyTeamReport.Domain;
using CM.WeeklyTeamReport.Domain.Dto;
using CM.WeeklyTeamReport.Domain.Dto.Implementations;
using CM.WeeklyTeamReport.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CM.WeeklyTeamReport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/members/{memberId}/reports")]
    public class WeeklyReportController : ControllerBase
    {
        private readonly IWeeklyReportManager _manager;

        public WeeklyReportController(IWeeklyReportManager weeklyReportManager)
        {
            _manager = weeklyReportManager;
        }

        [HttpGet]
        [Route("current-reports")]
        public IActionResult GetTeamReports([FromQuery(Name = "team")] string team, [FromQuery(Name = "week")] string week, 
            int companyId, int memberId)
        {
            var result = _manager.ReadReportHistory(companyId, memberId, team, week);          
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("old-reports")]
        public IActionResult GetOldReports([FromQuery(Name = "team")] string team, [FromQuery(Name = "filter")] string filter, 
            int companyId, int memberId)
        {
            var averageReport = _manager.ReadAverageOldReports(companyId, memberId, team, filter);
            var individualReports = _manager.ReadIndividualOldReports(companyId, memberId, team, filter);

            if (averageReport == null || individualReports == null)
            {
                return NotFound();
            }
            var result = new SummaryOldReport()
            {
                AverageOldReportDto = averageReport,
                OverviewReportsDtos = individualReports
            };
            return Ok(result);
        }
        [HttpGet]
        public IActionResult Get(int companyId, int memberId)
        {
            var result = _manager.readAll(companyId, memberId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("{reportId}")]
        public IActionResult Get(int companyId, int memberId, int reportId)
        {
            var result = _manager.read(companyId, memberId, reportId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("extended/{start}/{end}")]
        public IActionResult GetExtendedInInterval(int companyId, int memberId, long start, long end)
        {
            DateTime startDate = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(start);
            DateTime endDate = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(end);
            var result = _manager.ReadReportsInInterval(companyId, memberId, startDate, endDate);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ReportsDto entity, int companyId, int memberId)
        {
            var result = _manager.create(entity);
            if (result == null)
            {
                return NoContent();
            }
            var uriCreatedReport = $"api/companies/{companyId}/members/{memberId}/reports/{result.ID}";
            return Created(uriCreatedReport, result);
        }

        [HttpPut]
        [Route("{reportId}")]
        public IActionResult Put([FromBody] ReportsDto entity, int companyId, int memberId, int reportId)
        {
            var updatedReport = _manager.read(companyId, memberId, reportId);
            if (updatedReport == null)
            {
                return NotFound();
            }
            _manager.update(updatedReport, entity);
            return NoContent();
        }

        [HttpDelete]
        [Route("{reportId}")]
        public IActionResult Delete(int companyId, int memberId, int reportId)
        {
            var result = _manager.read(companyId, memberId, reportId);
            if (result == null)
            {
                return NotFound();
            }
            _manager.delete(result);
            return NoContent();
        }
    }
}
