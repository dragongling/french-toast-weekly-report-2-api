﻿using CM.WeeklyTeamReport.Domain;
using CM.WeeklyTeamReport.Domain.Dto.Implementations;
using CM.WeeklyTeamReport.Domain.Dto.Interfaces;
using CM.WeeklyTeamReport.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public IActionResult Post([FromBody] IWeeklyReportDto entity, int companyId, int memberId)
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
        public IActionResult Put([FromBody] IWeeklyReportDto entity, int companyId, int memberId, int reportId)
        {
            var updatedReport = _manager.read(companyId, memberId, reportId);
            if (updatedReport == null)
            {
                return NoContent();
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
            _manager.delete(companyId, memberId, reportId);
            return NoContent();
        }
    }
}