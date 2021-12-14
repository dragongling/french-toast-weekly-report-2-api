using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using CM.WeeklyTeamReport.Domain.Entities.Interfaces;

namespace CM.WeeklyTeamReport.Domain.IntegrationTests
{
    public class TeamMemberRepositoryTests
    {
        [Fact]
        public void ShouldPerformBasicCRUD()
        {
            var teamMemberRepo = new TeamMemberRepository(SetupTests.Configuration);
            var companyRepo = new CompanyRepository(SetupTests.Configuration);
            var company = companyRepo.Create(new Company { Name = "Test company" });
            var teamMember = teamMemberRepo.Create(new TeamMember
            {
                FirstName = "Anatoliy",
                LastName = "Kolodkin",
                Title = "CEO",
                Email = "akolodkin@ankocorp.com",
                CompanyId = company.ID
            });
            teamMember.ID.Should().NotBe(0);
            var readTeamMember = teamMemberRepo.Read(teamMember.ID);
            readTeamMember.FirstName.Should().Be(teamMember.FirstName);
            readTeamMember.LastName.Should().Be(teamMember.LastName);
            readTeamMember.Title.Should().Be(teamMember.Title);
            readTeamMember.Email.Should().Be(teamMember.Email);
            var newMail = "newmail@ankocorp.com";
            teamMember.Email = newMail;
            teamMemberRepo.Update(teamMember);
            readTeamMember = teamMemberRepo.Read(teamMember.ID);
            readTeamMember.Email.Should().Be(newMail);
            teamMemberRepo.Delete(teamMember.ID);
            readTeamMember = teamMemberRepo.Read(teamMember.ID);
            readTeamMember.Should().BeNull();
            companyRepo.Delete(company);
        }

        [Fact]
        public void ShouldReadAll()
        {
            var repository = new TeamMemberRepository(SetupTests.Configuration);
            var result = repository.ReadAll();
            result.Should().AllBeOfType<TeamMember>();
        }

        [Fact]
        public void ShouldDeleteWithRelations()
        {

        }

        [Fact]
        public void ShouldHandleReportingMembers()
        {
            var companyRepo = new CompanyRepository(SetupTests.Configuration);
            var company = companyRepo.Create(new Company { Name = "Test company" });
            var tmRepo = new TeamMemberRepository(SetupTests.Configuration);
            var teamMember = tmRepo.Create(
                GetTeamMember(company)
            );
            var reportingMembers = new List<ITeamMember>()
            {
                tmRepo.Create(GetTeamMember(company, 2)),
                tmRepo.Create(GetTeamMember(company, 3))
            };
            var leadersToReport = new List<ITeamMember>()
            {
                tmRepo.Create(GetTeamMember(company, 4)),
                tmRepo.Create(GetTeamMember(company, 5))
            };

            tmRepo.GetReportingMembers(teamMember).Should().BeEmpty();
            tmRepo.GetLeadersToReport(teamMember).Should().BeEmpty();

            tmRepo.AddReportingMember(reportingMembers[0], teamMember);
            tmRepo.AddReportingMember(reportingMembers[1], teamMember);
            tmRepo.AddReportingMember(teamMember, leadersToReport[0]);
            tmRepo.AddReportingMember(teamMember, leadersToReport[1]);

            tmRepo.GetReportingMembers(teamMember).Should().BeEquivalentTo(reportingMembers);
            tmRepo.GetLeadersToReport(teamMember).Should().BeEquivalentTo(leadersToReport);

            tmRepo.RemoveReportingMember(reportingMembers[0], teamMember);
            tmRepo.RemoveReportingMember(reportingMembers[1], teamMember);
            tmRepo.RemoveReportingMember(teamMember, leadersToReport[0]);
            tmRepo.RemoveReportingMember(teamMember, leadersToReport[1]);

            tmRepo.GetReportingMembers(teamMember).Should().BeEmpty();
            tmRepo.GetLeadersToReport(teamMember).Should().BeEmpty();

            foreach (var tm in reportingMembers)
                tmRepo.Delete(tm);
            foreach (var tm in leadersToReport)
                tmRepo.Delete(tm);
            tmRepo.Delete(teamMember);
            companyRepo.Delete(company);
        }

        private static ITeamMember GetTeamMember(ICompany company, int seed = 1)
        {
            return new TeamMember
            {
                FirstName = $"F{seed}",
                LastName = $"L{seed}",
                Title = $"T{seed}",
                Email = $"mail{seed}@company{company.ID}.com",
                CompanyId = company.ID
            };
        }
    }
}
