using allofthesestarshaveareason.Components;
using allofthesestarshaveareason.Services;
using allofthesestarshaveareason.Services.Implementations;
using allofthesestarshaveareason.Services.Interfaces;
using allofthesestarshaveareason.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=analysis.db";
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseSqlite(connectionString));
    
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // Blazor Server circuit options
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        options.DisconnectedCircuitMaxRetained = 100;
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    });

builder.Services.AddMudServices();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

// Services
builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSingleton<IFFmpegService, FFmpegService>();
builder.Services.AddSingleton<ITranscriptService, WhisperTranscriptService>();
builder.Services.AddSingleton<ISceneDetectionService, OpenCvSceneDetectionService>();
builder.Services.AddSingleton<ITextAnalysisService, OnnxTextAnalysisService>();
builder.Services.AddScoped<IJobRepository, EfJobRepository>();
builder.Services.AddScoped<IAnalysisService, allofthesestarshaveareason.Services.VideoAnalysisOrchestrator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    dbContext.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


