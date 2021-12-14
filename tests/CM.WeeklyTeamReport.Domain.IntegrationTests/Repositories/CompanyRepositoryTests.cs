using CM.WeeklyTeamReport.Domain.Entities.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CM.WeeklyTeamReport.Domain.IntegrationTests
{
    public class CompanyRepositoryTests
    {
        [Fact]
        public void ShouldPerformBasicCRUD()
        {
            var repository = new CompanyRepository(SetupTests.Configuration);
            var company = repository.Create(new Company { Name = "Test company" });
            company.ID.Should().NotBe(0);
            var readCompany = repository.Read(company.ID);
            readCompany.ID.Should().Be(company.ID);
            readCompany.Name.Should().Be(company.Name);
            readCompany.CreationDate.Should().Be(company.CreationDate);
            var newName = "New name";
            company.Name = newName;
            repository.Update(company);
            readCompany = repository.Read(company.ID);
            readCompany.Name.Should().Be(newName);
            repository.Delete(company);
            readCompany = repository.Read(company.ID);
            readCompany.Should().BeNull();
        }

        [Fact]
        public void ShouldReadAll()
        {
            var repository = new CompanyRepository(SetupTests.Configuration);
            var result = repository.ReadAll();
            result.Should().AllBeOfType<Company>();
        }
    }
}
