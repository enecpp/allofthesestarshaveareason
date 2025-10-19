using allofthesestarshaveareason.Components;
using allofthesestarshaveareason.Services;
using allofthesestarshaveareason.Services.Interfaces;
using allofthesestarshaveareason.Services.Implementations;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddSingleton<IJobRepository, InMemoryJobRepository>();
builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSingleton<IFFmpegService, FFmpegService>();
builder.Services.AddSingleton<ITranscriptService, WhisperTranscriptService>();
builder.Services.AddSingleton<ISceneDetectionService, OpenCvSceneDetectionService>();

builder.Services.AddScoped<IAnalysisService, VideoAnalysisOrchestrator>();

var app = builder.Build();

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


