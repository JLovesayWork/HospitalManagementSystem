﻿using System.Net;
using FluentAssertions;
using HospitalManagementSystem.DoctorTests.Unit_Tests.Factories;
using HospitalManagementSystem.DoctorTests.Unit_Tests.Controllers;

namespace HospitalManagementSystem.DoctorTests
{
    [Collection("ApiWebApplicationFactory")]
    public class DoctorControllerTests : DoctorControllerTestsBase
    {
        public DoctorControllerTests(ApiWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task WhenGetBroadcastJobsByQuery_InvalidPageSizeLessThanOne_ThenExpectedResult()
        {
            var response = await Client.GetAsync($"/api/Doctors/doctors?page=0&pagesize=-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();

            result.Should().BeEquivalentTo($"The BroadcastJobQuery is invalid.{Environment.NewLine}1 error(s) found:{Environment.NewLine}[\"'Page Size' must be greater than '0'.\"]");
        }

    }
}
