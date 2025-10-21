# CustomVideoPlayer Component

## ?? Genel Bak��

`CustomVideoPlayer.razor` Blazor i�in tasarlanm��, modern ve yeniden kullan�labilir bir video oynat�c� bile�enidir. JavaScript Interop kullanarak g��l� video kontrol �zellikleri sunar.

## ? �zellikler

- ? **Zaman Kontrol�**: Video'yu istedi�iniz zaman damgas�na atlama
- ? **Oynatma Kontrolleri**: Play, Pause, Seek
- ? **Event Callbacks**: Video olaylar�n� dinleme (play, pause, timeupdate, vb.)
- ? **Otomatik Dispose**: Memory leak �nleme
- ? **Hata Y�netimi**: Try-catch bloklar� ile g�venli �al��ma
- ? **TypeScript Benzeri Mod�l Sistemi**: ES6 import/export kullan�m�
- ? **Responsive Tasar�m**: Mobil uyumlu CSS

## ?? Kurulum

### 1. Dosya Yap�s�

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
    ??? VideoResult.razor (�rnek kullan�m)
```

### 2. JavaScript Dosyas�n� Ekleyin

`wwwroot/js/videoInterop.js` dosyas�n�n do�ru konumda oldu�undan emin olun.

## ?? Kullan�m

### Temel Kullan�m

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

### Event Callback'leri ile Kullan�m

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
| `VideoUrl` | `string?` | `null` | Video dosyas�n�n URL'si |
| `OnTimeChanged` | `EventCallback<double>` | - | Video zaman� de�i�ti�inde tetiklenir |
| `OnDurationLoaded` | `EventCallback<double>` | - | Video s�resi y�klendi�inde tetiklenir |
| `OnVideoPlay` | `EventCallback` | - | Video oynat�lmaya ba�lad���nda |
| `OnVideoPause` | `EventCallback` | - | Video duraklat�ld���nda |
| `OnVideoEnded` | `EventCallback` | - | Video bitti�inde |

### Public Methods

| Method | Return Type | Parameters | Description |
|--------|-------------|------------|-------------|
| `SeekTo(double)` | `Task` | `timeInSeconds` | Video'yu belirtilen zamana atlar |
| `Play()` | `Task` | - | Video'yu oynat�r |
| `Pause()` | `Task` | - | Video'yu duraklatt�r |
| `GetCurrentTime()` | `Task<double>` | - | Mevcut oynatma zaman�n� d�nd�r�r |
| `GetDuration()` | `Task<double>` | - | Video s�resini d�nd�r�r |

## ?? JavaScript Interop API

`videoInterop.js` dosyas�ndaki t�m fonksiyonlar:

```javascript
// Temel Kontroller
seekTo(videoElement, timeInSeconds)
play(videoElement)
pause(videoElement)
getCurrentTime(videoElement)
getDuration(videoElement)

// Geli�mi� �zellikler
setVolume(videoElement, volume)          // 0-1 aras�
getVolume(videoElement)
setPlaybackRate(videoElement, rate)      // 0.25-2.0 aras�
getPlaybackRate(videoElement)
isPlaying(videoElement)
toggleFullscreen(videoElement)
```

## ?? CSS Customization

`CustomVideoPlayer.razor.css` dosyas�n� d�zenleyerek g�r�n�m� �zelle�tirebilirsiniz:

```css
.video-player-container {
    border-radius: 12px;  /* Kenarl�klar� yuvarlatma */
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);  /* G�lge efekti */
}
```

## ?? �nemli Notlar

1. **JS Module Import**: Blazor Server kullan�yorsan�z, `IJSRuntime.InvokeAsync<IJSObjectReference>("import", ...)` .NET 5+ ile kullan�labilir.

2. **Video URL'si**: Video dosyas�n�n CORS ayarlar�n�n do�ru yap�land�r�ld���ndan emin olun.

3. **Dispose Pattern**: Bile�en `IAsyncDisposable` implement eder, bu nedenle kullan�mdan sonra otomatik temizlik yapar.

4. **Error Handling**: T�m JavaScript �a�r�lar� try-catch bloklar�yla korunmu�tur.

## ?? �rnek Senaryo: Video Analiz Sistemi

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

### "Module not found" hatas�

```bash
# wwwroot/js/videoInterop.js dosyas�n�n do�ru konumda oldu�unu kontrol edin
# Browser Console'da �u �ekilde test edin:
import('./js/videoInterop.js').then(m => console.log(m));
```

### Video oynat�lm�yor

```csharp
// Video URL'sinin eri�ilebilir oldu�unu kontrol edin
// CORS hatalar�n� browser console'dan inceleyin
```

### Seek �al��m�yor

```csharp
// Video'nun metadata's�n�n y�klendi�inden emin olun
// OnDurationLoaded callback'ini kullanarak kontrol edin
```

## ?? Lisans

MIT License - Projede �zg�rce kullanabilirsiniz.

## ?? Katk�da Bulunma

Pull request'ler kabul edilir. B�y�k de�i�iklikler i�in l�tfen �nce bir issue a��n.

---

**Not**: Bu bile�en production-ready olup, modern Blazor uygulamalar�nda g�venle kullan�labilir.
