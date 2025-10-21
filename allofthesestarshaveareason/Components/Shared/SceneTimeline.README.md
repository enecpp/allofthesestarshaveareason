# SceneTimeline Component

## ?? Genel Bak��

`SceneTimeline.razor` video sahnelerini g�rsel bir zaman �izelgesi olarak g�r�nt�leyen interaktif bir Blazor bile�enidir. Sahneleri video s�resine g�re orant�l� olarak renkli �ubuklar halinde g�sterir ve kullan�c�n�n sahne ba�lang�� zamanlar�na t�klayarak videoda gezinmesini sa�lar.

## ? �zellikler

- ? **Orant�l� G�rselle�tirme**: Sahneler, s�relerine g�re orant�l� geni�likte g�sterilir
- ? **Renkli Timeline**: Her sahne farkl� renkle vurgulan�r (10 renk paleti)
- ? **Hover Tooltips**: Sahne �zerine gelindi�inde detayl� bilgi g�sterilir
- ? **T�klanabilir Sahneler**: Bir sahneye t�klayarak video'da o zamana atlay�n
- ? **Detayl� Liste G�r�n�m�**: Geni�letilebilir panel ile t�m sahne detaylar�
- ? **�ift Tip Deste�i**: Hem `Scene` (Model) hem `SceneDto` (DTO) destekler
- ? **Responsive Tasar�m**: Mobil ve masa�st� uyumlu
- ? **Animasyonlar**: Smooth hover ve click efektleri
- ? **Dark Mode**: Otomatik dark mode deste�i

## ?? Kurulum

Dosyalar zaten projede mevcut:
```
allofthesestarshaveareason/Components/Shared/SceneTimeline.razor
allofthesestarshaveareason/Components/Shared/SceneTimeline.razor.css
```

## ?? Kullan�m

### Temel Kullan�m (Model ile)

```razor
@page "/timeline-demo"
@using allofthesestarshaveareason.Models

<SceneTimeline Scenes="@sceneList" OnSceneClicked="HandleSceneClick" />

@code {
    private List<Scene> sceneList = new()
    {
        new Scene 
        { 
            Title = "Giri�", 
            StartTime = 0, 
            EndTime = 30 
        },
        new Scene 
        { 
            Title = "Ana Konu", 
            StartTime = 30, 
            EndTime = 120 
        },
        new Scene 
        { 
            Title = "Sonu�", 
            StartTime = 120, 
            EndTime = 180 
        }
    };

    private void HandleSceneClick(double time)
    {
        Console.WriteLine($"Clicked scene at: {time}s");
    }
}
```

### DTO ile Kullan�m

```razor
@using allofthosestarshaveareason.DTOs

<SceneTimeline DtoScenes="@dtoScenes" OnSceneClicked="JumpToScene" />

@code {
    private List<SceneDto>? dtoScenes;

    protected override async Task OnInitializedAsync()
    {
        // API'den DTO'lar� y�kle
        var result = await Http.GetFromJsonAsync<VideoAnalysisResultDto>(
            "api/VideoAnalysis/result/123");
        dtoScenes = result?.Scenes;
    }

    private async Task JumpToScene(double time)
    {
        // Video player'a atla
    }
}
```

### CustomVideoPlayer ile Tam Entegrasyon

```razor
@page "/video-with-timeline"

<MudGrid>
    <MudItem xs="12">
        <CustomVideoPlayer @ref="videoPlayer" 
                         VideoUrl="@videoUrl"
                         OnDurationLoaded="OnVideoDurationLoaded" />
    </MudItem>
    
    <MudItem xs="12">
        <SceneTimeline DtoScenes="@scenes" OnSceneClicked="JumpToTime" />
    </MudItem>
</MudGrid>

@code {
    private CustomVideoPlayer? videoPlayer;
    private string videoUrl = "https://example.com/video.mp4";
    private List<SceneDto>? scenes;
    private double videoDuration;

    protected override async Task OnInitializedAsync()
    {
        scenes = await LoadScenesFromApi();
    }

    private void OnVideoDurationLoaded(double duration)
    {
        videoDuration = duration;
        Console.WriteLine($"Video duration: {duration}s");
    }

    private async Task JumpToTime(double time)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(time);
            await videoPlayer.Play();
        }
    }

    private async Task<List<SceneDto>> LoadScenesFromApi()
    {
        // API implementation
        return new List<SceneDto>();
    }
}
```

## ?? API Reference

### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `Scenes` | `List<Scene>?` | No* | `null` | Model tipinde sahne listesi |
| `DtoScenes` | `List<SceneDto>?` | No* | `null` | DTO tipinde sahne listesi |
| `OnSceneClicked` | `EventCallback<double>` | No | - | Sahneye t�kland���nda tetiklenir, ba�lang�� zaman�n� d�nd�r�r |

\* `Scenes` veya `DtoScenes`'den en az biri sa�lanmal�d�r.

### Scene Model Yap�s�

```csharp
public class Scene
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

### SceneDto Yap�s�

```csharp
public class SceneDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }
}
```

### Methods

| Method | Return Type | Parameters | Description |
|--------|-------------|------------|-------------|
| `SceneClicked` | `Task` | `double time` | Private - Sahne t�klama olay�n� i�ler |
| `GetSceneWidthPercentage` | `double` | `object scene` | Sahnenin timeline'daki geni�lik y�zdesini hesaplar |
| `GetSceneColor` | `string` | `int index` | Sahne i�in renk d�nd�r�r |
| `FormatTime` | `string` | `double seconds` | Saniyeyi `mm:ss` format�na �evirir |
| `FormatDuration` | `string` | `double seconds` | S�reyi okunabilir formata �evirir (�rn: "2dk 30sn") |

## ?? Renk Paleti

Bile�en otomatik olarak 10 farkl� renk kullan�r:

```csharp
private readonly string[] sceneColors = new[]
{
    "#1e88e5", // Blue
    "#43a047", // Green
    "#fb8c00", // Orange
    "#8e24aa", // Purple
    "#e53935", // Red
    "#00acc1", // Cyan
    "#fdd835", // Yellow
    "#3949ab", // Indigo
    "#00897b", // Teal
    "#f4511e"  // Deep Orange
};
```

## ?? G�rsel �zellikler

### Timeline Bar
- **Orant�l� Geni�lik**: Her sahne, s�resine g�re orant�l� geni�likte g�r�nt�lenir
- **Gradient Background**: Derinlik hissi i�in gradient arka plan
- **Border**: Sahneler aras� ay�r�c� �izgiler
- **Hover Effect**: Sahne �zerine gelindi�inde b�y�me animasyonu

### Hover Tooltip
Sahne �zerine gelindi�inde g�sterilir:
- Sahne ba�l���
- Ba�lang�� - Biti� zaman�
- Toplam s�re

### Detayl� Liste G�r�n�m�
Geni�letilebilir panel i�inde:
- Numaraland�r�lm�� sahne listesi
- Her sahne i�in play butonu
- Renkli etiketler
- T�klanabilir sat�rlar

## ?? Tam Video Analiz Sistemi �rne�i

```razor
@page "/analysis/{ResultId:int}"
@using allofthosestarshaveareason.DTOs
@inject HttpClient Http

<MudContainer MaxWidth="MaxWidth.ExtraLarge">
    <MudGrid>
        <!-- Video Player -->
        <MudItem xs="12" lg="8">
            <MudPaper Class="pa-4">
                <CustomVideoPlayer @ref="videoPlayer" 
                                 VideoUrl="@result?.VideoUrl"
                                 OnTimeChanged="OnTimeChanged"
                                 OnDurationLoaded="OnDurationLoaded" />
                
                <MudText Class="mt-2">
                    @FormatTime(currentTime) / @FormatTime(duration)
                </MudText>
            </MudPaper>

            <!-- Scene Timeline -->
            <MudPaper Class="pa-4 mt-4">
                <SceneTimeline DtoScenes="@result?.Scenes" 
                             OnSceneClicked="JumpToScene" />
            </MudPaper>
        </MudItem>

        <!-- Transcript -->
        <MudItem xs="12" lg="4">
            <MudPaper Class="pa-4" Style="height: 100%;">
                <TranscriptView DtoSegments="@result?.Transcript" 
                              OnSegmentClicked="JumpToSegment" />
            </MudPaper>
        </MudItem>
    </MudGrid>
</MudContainer>

@code {
    [Parameter]
    public int ResultId { get; set; }

    private CustomVideoPlayer? videoPlayer;
    private VideoAnalysisResultDto? result;
    private double currentTime;
    private double duration;

    protected override async Task OnInitializedAsync()
    {
        result = await Http.GetFromJsonAsync<VideoAnalysisResultDto>(
            $"api/VideoAnalysis/result/{ResultId}");
    }

    private async Task JumpToScene(double time)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(time);
            await videoPlayer.Play();
        }
    }

    private async Task JumpToSegment(double time)
    {
        await JumpToScene(time);
    }

    private void OnTimeChanged(double time)
    {
        currentTime = time;
    }

    private void OnDurationLoaded(double dur)
    {
        duration = dur;
    }

    private string FormatTime(double seconds)
    {
        return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
    }
}
```

## ?? �zelle�tirme

### CSS Customization

`SceneTimeline.razor.css` dosyas�n� d�zenleyerek g�r�n�m� �zelle�tirebilirsiniz:

```css
/* Timeline y�ksekli�ini de�i�tir */
.scene-timeline-container {
    height: 80px; /* Varsay�lan: 60px */
}

/* Hover efektini g��lendir */
.scene-segment:hover {
    transform: scaleY(1.15); /* Varsay�lan: 1.05 */
}

/* Tooltip stilini de�i�tir */
.scene-tooltip {
    background: rgba(33, 150, 243, 0.95); /* Mavi ton */
}

/* Renk paletini �zelle�tir */
@code {
    private readonly string[] sceneColors = new[]
    {
        "#ff6b6b", "#4ecdc4", "#45b7d1", "#f9ca24", "#6c5ce7"
    };
}
```

### Aktif Sahne Vurgulama

Mevcut video zaman�na g�re aktif sahneyi vurgulayabilirsiniz:

```razor
<SceneTimeline DtoScenes="@scenes" 
               OnSceneClicked="JumpToScene"
               CurrentTime="@currentVideoTime" />

@code {
    // CSS'e active class ekleyin:
    // .scene-segment.active {
    //     box-shadow: inset 0 -4px 0 yellow;
    // }
}
```

## ?? �nemli Notlar

1. **Sahne S�releri**: Sahnelerin `StartTime` ve `EndTime` de�erleri do�ru olmal�d�r. �ak��an veya eksik zaman aral�klar� g�rsel sorunlara neden olabilir.

2. **Toplam S�re**: Toplam video s�resi, sahnelerin son `EndTime` de�erinden otomatik hesaplan�r.

3. **Minimum Geni�lik**: �ok k�sa sahneler (<%5) i�in minimum geni�lik uygulan�r ve metin gizlenir (sadece tooltip g�sterilir).

4. **Performance**: �ok fazla sahne (>50) olmas� durumunda performans sorunlar� ya�anabilir. Bu durumda sahneleri gruplamay� d���n�n.

5. **Null Safety**: `Scenes` veya `DtoScenes` parametrelerinden en az biri `null` olmayan bir de�er i�ermelidir.

## ?? Troubleshooting

### Timeline g�r�nm�yor

```csharp
// Sahnelerin do�ru y�klendi�inden emin olun
@code {
    protected override void OnInitialized()
    {
        Console.WriteLine($"Scenes count: {Scenes?.Count ?? 0}");
        Console.WriteLine($"DTO Scenes count: {DtoScenes?.Count ?? 0}");
    }
}
```

### Sahneler yanl�� geni�likte

```csharp
// EndTime de�erlerinin do�ru oldu�undan emin olun
// StartTime < EndTime olmal�
@code {
    protected override void OnInitialized()
    {
        foreach (var scene in Scenes)
        {
            if (scene.EndTime <= scene.StartTime)
            {
                Console.WriteLine($"Invalid scene: {scene.Title}");
            }
        }
    }
}
```

### OnSceneClicked tetiklenmiyor

```csharp
// EventCallback'in do�ru tan�mland���ndan emin olun
<SceneTimeline Scenes="@scenes" 
               OnSceneClicked="HandleClick" />  <!-- Method ad� do�ru mu? -->

@code {
    private async Task HandleClick(double time)
    {
        Console.WriteLine($"Clicked at: {time}");
    }
}
```

## ?? Best Practices

1. **Sahne Ba�l�klar�**: K�sa ve a��klay�c� ba�l�klar kullan�n (max 20 karakter �nerilir)
2. **Sahne Say�s�**: Optimal g�r�n�m i�in 3-20 aras� sahne kullan�n
3. **Zaman Aral�klar�**: Sahnelerin �ak��mad���ndan emin olun
4. **Video Player Senkronizasyonu**: Timeline ile video player'� her zaman senkronize tutun
5. **Loading States**: API'den veri y�klenirken loading indicator g�sterin

## ?? �rnek Senaryolar

### 1. E�itim Videolar�
```
Giri� (0-30s) ? Kavramlar (30-120s) ? Uygulama (120-300s) ? �zet (300-360s)
```

### 2. Film Analizi
```
Opening Credits ? Act 1 ? Act 2 ? Climax ? Resolution ? End Credits
```

### 3. Sunum Videolar�
```
Ba�l�k ? Problem ? ��z�m ? Demo ? Sonu� ? Sorular
```

## ?? �lgili Bile�enler

- **CustomVideoPlayer.razor** - Video oynat�c�
- **TranscriptView.razor** - Transkript g�r�nt�leyici
- **VideoResult.razor** - Analiz sonu� sayfas�

## ?? Lisans

MIT License - Projede �zg�rce kullanabilirsiniz.

---

**Not**: Bu bile�en, video analiz sistemlerinde sahneleri g�rsel olarak temsil etmek i�in ideal bir ��z�md�r. CustomVideoPlayer ve TranscriptView ile birlikte kullan�ld���nda tam bir video navigasyon deneyimi sunar.
