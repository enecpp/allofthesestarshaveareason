# CustomVideoPlayer Component

## ?? Genel Bakýþ

`CustomVideoPlayer.razor` Blazor için tasarlanmýþ, modern ve yeniden kullanýlabilir bir video oynatýcý bileþenidir. JavaScript Interop kullanarak güçlü video kontrol özellikleri sunar.

## ? Özellikler

- ? **Zaman Kontrolü**: Video'yu istediðiniz zaman damgasýna atlama
- ? **Oynatma Kontrolleri**: Play, Pause, Seek
- ? **Event Callbacks**: Video olaylarýný dinleme (play, pause, timeupdate, vb.)
- ? **Otomatik Dispose**: Memory leak önleme
- ? **Hata Yönetimi**: Try-catch bloklarý ile güvenli çalýþma
- ? **TypeScript Benzeri Modül Sistemi**: ES6 import/export kullanýmý
- ? **Responsive Tasarým**: Mobil uyumlu CSS

## ?? Kurulum

### 1. Dosya Yapýsý

```
allofthesestarshaveareason/
??? Components/
?   ??? Shared/
?       ??? CustomVideoPlayer.razor
?       ??? CustomVideoPlayer.razor.css
??? wwwroot/
?   ??? js/
?       ??? videoInterop.js
??? Components/Pages/
    ??? VideoResult.razor (örnek kullaným)
```

### 2. JavaScript Dosyasýný Ekleyin

`wwwroot/js/videoInterop.js` dosyasýnýn doðru konumda olduðundan emin olun.

## ?? Kullaným

### Temel Kullaným

```razor
@page "/video-demo"

<CustomVideoPlayer @ref="player" 
                   VideoUrl="https://example.com/video.mp4" />

<MudButton OnClick="SeekTo30Seconds">30. Saniyeye Git</MudButton>

@code {
    private CustomVideoPlayer? player;

    private async Task SeekTo30Seconds()
    {
        if (player != null)
        {
            await player.SeekTo(30);
        }
    }
}
```

### Event Callback'leri ile Kullaným

```razor
<CustomVideoPlayer VideoUrl="@videoUrl"
                   OnTimeChanged="HandleTimeChange"
                   OnDurationLoaded="HandleDurationLoaded"
                   OnVideoPlay="HandlePlay"
                   OnVideoPause="HandlePause"
                   OnVideoEnded="HandleEnded" />

@code {
    private string videoUrl = "https://example.com/video.mp4";
    private double currentTime;
    private double duration;

    private void HandleTimeChange(double time)
    {
        currentTime = time;
        Console.WriteLine($"Current time: {time}s");
    }

    private void HandleDurationLoaded(double dur)
    {
        duration = dur;
        Console.WriteLine($"Video duration: {dur}s");
    }

    private void HandlePlay()
    {
        Console.WriteLine("Video started playing");
    }

    private void HandlePause()
    {
        Console.WriteLine("Video paused");
    }

    private void HandleEnded()
    {
        Console.WriteLine("Video ended");
    }
}
```

### Transcript Segmentine Atlama (VideoAnalysis Senaryosu)

```razor
<CustomVideoPlayer @ref="videoPlayer" VideoUrl="@result.VideoUrl" />

<MudList>
    @foreach (var segment in transcript)
    {
        <MudListItem OnClick="() => JumpToSegment(segment.StartTime)">
            <MudText>@segment.Speaker: @segment.Text</MudText>
            <MudText Typo="Typo.caption">@FormatTime(segment.StartTime)</MudText>
        </MudListItem>
    }
</MudList>

@code {
    private CustomVideoPlayer? videoPlayer;

    private async Task JumpToSegment(double startTime)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(startTime);
            await videoPlayer.Play();
        }
    }

    private string FormatTime(double seconds)
    {
        return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
    }
}
```

## ?? API Reference

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `VideoUrl` | `string?` | `null` | Video dosyasýnýn URL'si |
| `OnTimeChanged` | `EventCallback<double>` | - | Video zamaný deðiþtiðinde tetiklenir |
| `OnDurationLoaded` | `EventCallback<double>` | - | Video süresi yüklendiðinde tetiklenir |
| `OnVideoPlay` | `EventCallback` | - | Video oynatýlmaya baþladýðýnda |
| `OnVideoPause` | `EventCallback` | - | Video duraklatýldýðýnda |
| `OnVideoEnded` | `EventCallback` | - | Video bittiðinde |

### Public Methods

| Method | Return Type | Parameters | Description |
|--------|-------------|------------|-------------|
| `SeekTo(double)` | `Task` | `timeInSeconds` | Video'yu belirtilen zamana atlar |
| `Play()` | `Task` | - | Video'yu oynatýr |
| `Pause()` | `Task` | - | Video'yu duraklattýr |
| `GetCurrentTime()` | `Task<double>` | - | Mevcut oynatma zamanýný döndürür |
| `GetDuration()` | `Task<double>` | - | Video süresini döndürür |

## ?? JavaScript Interop API

`videoInterop.js` dosyasýndaki tüm fonksiyonlar:

```javascript
// Temel Kontroller
seekTo(videoElement, timeInSeconds)
play(videoElement)
pause(videoElement)
getCurrentTime(videoElement)
getDuration(videoElement)

// Geliþmiþ Özellikler
setVolume(videoElement, volume)          // 0-1 arasý
getVolume(videoElement)
setPlaybackRate(videoElement, rate)      // 0.25-2.0 arasý
getPlaybackRate(videoElement)
isPlaying(videoElement)
toggleFullscreen(videoElement)
```

## ?? CSS Customization

`CustomVideoPlayer.razor.css` dosyasýný düzenleyerek görünümü özelleþtirebilirsiniz:

```css
.video-player-container {
    border-radius: 12px;  /* Kenarlýklarý yuvarlatma */
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);  /* Gölge efekti */
}
```

## ?? Önemli Notlar

1. **JS Module Import**: Blazor Server kullanýyorsanýz, `IJSRuntime.InvokeAsync<IJSObjectReference>("import", ...)` .NET 5+ ile kullanýlabilir.

2. **Video URL'si**: Video dosyasýnýn CORS ayarlarýnýn doðru yapýlandýrýldýðýndan emin olun.

3. **Dispose Pattern**: Bileþen `IAsyncDisposable` implement eder, bu nedenle kullanýmdan sonra otomatik temizlik yapar.

4. **Error Handling**: Tüm JavaScript çaðrýlarý try-catch bloklarýyla korunmuþtur.

## ?? Örnek Senaryo: Video Analiz Sistemi

```razor
@page "/analysis/{ResultId:int}"
@inject HttpClient Http

<CustomVideoPlayer @ref="player" VideoUrl="@videoUrl" />

<MudTextField @bind-Value="searchQuery" Label="Videoda Ara" />
<MudButton OnClick="Search">Ara</MudButton>

@if (searchResults != null)
{
    <MudList>
        @foreach (var result in searchResults)
        {
            <MudListItem OnClick="() => JumpTo(result.StartTime)">
                @result.Text
            </MudListItem>
        }
    </MudList>
}

@code {
    [Parameter] public int ResultId { get; set; }
    
    private CustomVideoPlayer? player;
    private string videoUrl = "";
    private string searchQuery = "";
    private List<TranscriptSegmentDto>? searchResults;

    protected override async Task OnInitializedAsync()
    {
        var result = await Http.GetFromJsonAsync<VideoAnalysisResultDto>(
            $"api/VideoAnalysis/result/{ResultId}");
        videoUrl = result?.VideoUrl ?? "";
    }

    private async Task Search()
    {
        searchResults = await Http.GetFromJsonAsync<List<TranscriptSegmentDto>>(
            $"api/VideoAnalysis/result/{ResultId}/search?query={searchQuery}");
    }

    private async Task JumpTo(double time)
    {
        if (player != null)
        {
            await player.SeekTo(time);
            await player.Play();
        }
    }
}
```

## ?? Troubleshooting

### "Module not found" hatasý

```bash
# wwwroot/js/videoInterop.js dosyasýnýn doðru konumda olduðunu kontrol edin
# Browser Console'da þu þekilde test edin:
import('./js/videoInterop.js').then(m => console.log(m));
```

### Video oynatýlmýyor

```csharp
// Video URL'sinin eriþilebilir olduðunu kontrol edin
// CORS hatalarýný browser console'dan inceleyin
```

### Seek çalýþmýyor

```csharp
// Video'nun metadata'sýnýn yüklendiðinden emin olun
// OnDurationLoaded callback'ini kullanarak kontrol edin
```

## ?? Lisans

MIT License - Projede özgürce kullanabilirsiniz.

## ?? Katkýda Bulunma

Pull request'ler kabul edilir. Büyük deðiþiklikler için lütfen önce bir issue açýn.

---

**Not**: Bu bileþen production-ready olup, modern Blazor uygulamalarýnda güvenle kullanýlabilir.
