using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CertiFlowApp.Services.Email;

public sealed class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ResendEmailSender(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage)
    {
        var apiKey = _configuration["RESEND_API_KEY"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "RESEND_API_KEY is not configured.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.resend.com/emails");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = JsonContent.Create(new
        {
            from = "CertiFlow <noreply@sarasjodin.se>",
            to = new[] { email },
            subject,
            html = htmlMessage
        });

        using var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }
}

