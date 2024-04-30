using HtmlToPdfApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.GetService<IHostApplicationLifetime>()!.ApplicationStopped.Register(async () =>
{
    await browser.DisposeAsync();
    playwright.Dispose();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/Convert", async ([FromBody] ConvertDto dto) =>
{
    var page = await browser.NewPageAsync(
        //new BrowserNewPageOptions { ViewportSize = new ViewportSize { Height = 1080, Width = 1920 } }
        );
    await page.SetContentAsync(dto.Html);

    var data = await page.PdfAsync(new PagePdfOptions
    {
        DisplayHeaderFooter = true,
        Format = "A4",
        Margin = new Margin
        {
            Bottom = "1.27cm",
            Top = "1.27cm",
            Left = "1.27cm",
            Right = "1.27cm"
        },

        HeaderTemplate = dto.Header,
        FooterTemplate = dto.Footer,
    });
    // 生成完成后关闭页面，否则对应的chrome进程会一直存在
    _ = page.CloseAsync();
    return Results.File(data, "application/pdf");
}).WithName("ConvertToPdf").WithOpenApi();

app.MapPost("/v2/Convert", async ([FromBody] ConvertV2Dto dto) =>
{
    var page = await browser.NewPageAsync(
        );
    await page.SetContentAsync(dto.Html);

    var data = await page.PdfAsync(dto.PagePdfOptions);
    // 生成完成后关闭页面，否则对应的chrome进程会一直存在
    _ = page.CloseAsync();
    return Results.File(data, "application/pdf");
}).WithName("ConvertToPdfV2").WithOpenApi();

app.Run();