using FamilyHubs.SharedKernel.Telemetry;
using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralUi.UnitTests;

public class WhenUsingConnectTelemetryPiiRedactor
{
    [Fact]
    public void ThenRedactDependencyTelemetry_ApiQueryString_ShouldRedact()
    {
        // Arrange
        var redactor = new ConnectTelemetryPiiRedactor();
        var dependencyTelemetry = new DependencyTelemetry
        {
            Name = "GET /api/services-simple",
            Data = "latitude=12.345&longitude=-67.890&other=data"
        };

        // Act
        redactor.Initialize(dependencyTelemetry);

        // Assert
        dependencyTelemetry.Data.Should().Be("latitude=REDACTED&longitude=REDACTED&other=data");
    }

    [Fact]
    public void ThenCallingPostcodesRedactDependencyTelemetry_ApiQueryString_ShouldRedact()
    {
        // Arrange
        var redactor = new ConnectTelemetryPiiRedactor();
        var dependencyTelemetry = new DependencyTelemetry
        {
            Name = "GET /postcodes/",
            Data = "latitude=12.345&longitude=-67.890&other=data"
        };

        // Act
        redactor.Initialize(dependencyTelemetry);

        // Assert
        dependencyTelemetry.Data.Should().Be("latitude=REDACTED&longitude=REDACTED&other=data");
    }

    [Fact]
    public void RedactTraceTelemetry_MessageAndProperties_ShouldRedact()
    {
        // Arrange
        var redactor = new ConnectTelemetryPiiRedactor();
        var traceTelemetry = new TraceTelemetry
        {
            Message = "latitude=12.345&other=message",
            Properties = { { "Scope", "postcode=12345" }, { "QueryString", "latitude=3.1&longitude=78.901" } }
        };

        // Act
        var modifiedTraceTelemetry = new TraceTelemetry
        {
            Message = traceTelemetry.Message,
            Timestamp = traceTelemetry.Timestamp
        };
        foreach (var property in traceTelemetry.Properties)
        {
            modifiedTraceTelemetry.Properties.Add(property);
        }
        redactor.Initialize(modifiedTraceTelemetry);

        // Assert
        modifiedTraceTelemetry.Message.Should().Be("latitude=REDACTED&other=message");
        modifiedTraceTelemetry.Properties.Should().HaveCount(2)
            .And.ContainKey("Scope").WhoseValue.Should().Be("postcode=REDACTED");

        modifiedTraceTelemetry.Properties.Should().HaveCount(2)
            .And.ContainKey("QueryString").WhoseValue.Should().Be("latitude=REDACTED&longitude=REDACTED");


    }

    [Fact]
    public void RedactRequestTelemetry_Url_ShouldRedact()
    {
        // Arrange
        var redactor = new ConnectTelemetryPiiRedactor();
        var requestTelemetry = new RequestTelemetry
        {
            Url = new Uri("https://example.com?latitude=12.345&postcode=12345")
        };

        // Act
        redactor.Initialize(requestTelemetry);

        // Assert
        requestTelemetry.Url.Should().Be(new Uri("https://example.com?latitude=REDACTED&postcode=REDACTED"));
    }
}
