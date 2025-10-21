# SceneTimeline Component

## ?? Genel Bakýþ

`SceneTimeline.razor` video sahnelerini görsel bir zaman çizelgesi olarak görüntüleyen interaktif bir Blazor bileþenidir. Sahneleri video süresine göre orantýlý olarak renkli çubuklar halinde gösterir ve kullanýcýnýn sahne baþlangýç zamanlarýna týklayarak videoda gezinmesini saðlar.

## ? Özellikler

- ? **Orantýlý Görselleþtirme**: Sahneler, sürelerine göre orantýlý geniþlikte gösterilir
- ? **Renkli Timeline**: Her sahne farklý renkle vurgulanýr (10 renk paleti)
- ? **Hover Tooltips**: Sahne üzerine gelindiðinde detaylý bilgi gösterilir
- ? **Týklanabilir Sahneler**: Bir sahneye týklayarak video'da o zamana atlayýn
- ? **Detaylý Liste Görünümü**: Geniþletilebilir panel ile tüm sahne detaylarý
- ? **Çift Tip Desteði**: Hem `Scene` (Model) hem `SceneDto` (DTO) destekler
- ? **Responsive Tasarým**: Mobil ve masaüstü uyumlu
- ? **Animasyonlar**: Smooth hover ve click efektleri
- ? **Dark Mode**: Otomatik dark mode desteði

## ?? Kurulum

Dosyalar zaten projede mevcut:
```
allofthesestarshaveareason/Components/Shared/SceneTimeline.razor
allofthesestarshaveareason/Components/Shared/SceneTimeline.razor.css
```

## ?? Kullaným

### Temel Kullaným (Model ile)

```razor
@page "/timeline-demo"
@using allofthesestarshaveareason.Models

<SceneTimeline Scenes="@sceneList" OnSceneClicked="HandleSceneClick" />

@code {
    private List<Scene> sceneList = new()
    {
        new Scene 
        { 
            Title = "Giriþ", 
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
            Title = "Sonuç", 
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

### DTO ile Kullaným

```razor
@using allofthosestarshaveareason.DTOs

<SceneTimeline DtoScenes="@dtoScenes" OnSceneClicked="JumpToScene" />

@code {
    private List<SceneDto>? dtoScenes;

    protected override async Task OnInitializedAsync()
    {
        // API'den DTO'larý yükle
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
| `OnSceneClicked` | `EventCallback<double>` | No | - | Sahneye týklandýðýnda tetiklenir, baþlangýç zamanýný döndürür |

\* `Scenes` veya `DtoScenes`'den en az biri saðlanmalýdýr.

### Scene Model Yapýsý

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

### SceneDto Yapýsý

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
| `SceneClicked` | `Task` | `double time` | Private - Sahne týklama olayýný iþler |
| `GetSceneWidthPercentage` | `double` | `object scene` | Sahnenin timeline'daki geniþlik yüzdesini hesaplar |
| `GetSceneColor` | `string` | `int index` | Sahne için renk döndürür |
| `FormatTime` | `string` | `double seconds` | Saniyeyi `mm:ss` formatýna çevirir |
| `FormatDuration` | `string` | `double seconds` | Süreyi okunabilir formata çevirir (örn: "2dk 30sn") |

## ?? Renk Paleti

Bileþen otomatik olarak 10 farklý renk kullanýr:

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

## ?? Görsel Özellikler

### Timeline Bar
- **Orantýlý Geniþlik**: Her sahne, süresine göre orantýlý geniþlikte görüntülenir
- **Gradient Background**: Derinlik hissi için gradient arka plan
- **Border**: Sahneler arasý ayýrýcý çizgiler
- **Hover Effect**: Sahne üzerine gelindiðinde büyüme animasyonu

### Hover Tooltip
Sahne üzerine gelindiðinde gösterilir:
- Sahne baþlýðý
- Baþlangýç - Bitiþ zamaný
- Toplam süre

### Detaylý Liste Görünümü
Geniþletilebilir panel içinde:
- Numaralandýrýlmýþ sahne listesi
- Her sahne için play butonu
- Renkli etiketler
- Týklanabilir satýrlar

## ?? Tam Video Analiz Sistemi Örneði

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

## ?? Özelleþtirme

### CSS Customization

`SceneTimeline.razor.css` dosyasýný düzenleyerek görünümü özelleþtirebilirsiniz:

```css
/* Timeline yüksekliðini deðiþtir */
.scene-timeline-container {
    height: 80px; /* Varsayýlan: 60px */
}

/* Hover efektini güçlendir */
.scene-segment:hover {
    transform: scaleY(1.15); /* Varsayýlan: 1.05 */
}

/* Tooltip stilini deðiþtir */
.scene-tooltip {
    background: rgba(33, 150, 243, 0.95); /* Mavi ton */
}

/* Renk paletini özelleþtir */
@code {
    private readonly string[] sceneColors = new[]
    {
        "#ff6b6b", "#4ecdc4", "#45b7d1", "#f9ca24", "#6c5ce7"
    };
}
```

### Aktif Sahne Vurgulama

Mevcut video zamanýna göre aktif sahneyi vurgulayabilirsiniz:

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

## ?? Önemli Notlar

1. **Sahne Süreleri**: Sahnelerin `StartTime` ve `EndTime` deðerleri doðru olmalýdýr. Çakýþan veya eksik zaman aralýklarý görsel sorunlara neden olabilir.

2. **Toplam Süre**: Toplam video süresi, sahnelerin son `EndTime` deðerinden otomatik hesaplanýr.

3. **Minimum Geniþlik**: Çok kýsa sahneler (<%5) için minimum geniþlik uygulanýr ve metin gizlenir (sadece tooltip gösterilir).

4. **Performance**: Çok fazla sahne (>50) olmasý durumunda performans sorunlarý yaþanabilir. Bu durumda sahneleri gruplamayý düþünün.

5. **Null Safety**: `Scenes` veya `DtoScenes` parametrelerinden en az biri `null` olmayan bir deðer içermelidir.

## ?? Troubleshooting

### Timeline görünmüyor

```csharp
// Sahnelerin doðru yüklendiðinden emin olun
@code {
    protected override void OnInitialized()
    {
        Console.WriteLine($"Scenes count: {Scenes?.Count ?? 0}");
        Console.WriteLine($"DTO Scenes count: {DtoScenes?.Count ?? 0}");
    }
}
```

### Sahneler yanlýþ geniþlikte

```csharp
// EndTime deðerlerinin doðru olduðundan emin olun
// StartTime < EndTime olmalý
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
// EventCallback'in doðru tanýmlandýðýndan emin olun
<SceneTimeline Scenes="@scenes" 
               OnSceneClicked="HandleClick" />  <!-- Method adý doðru mu? -->

@code {
    private async Task HandleClick(double time)
    {
        Console.WriteLine($"Clicked at: {time}");
    }
}
```

## ?? Best Practices

1. **Sahne Baþlýklarý**: Kýsa ve açýklayýcý baþlýklar kullanýn (max 20 karakter önerilir)
2. **Sahne Sayýsý**: Optimal görünüm için 3-20 arasý sahne kullanýn
3. **Zaman Aralýklarý**: Sahnelerin çakýþmadýðýndan emin olun
4. **Video Player Senkronizasyonu**: Timeline ile video player'ý her zaman senkronize tutun
5. **Loading States**: API'den veri yüklenirken loading indicator gösterin

## ?? Örnek Senaryolar

### 1. Eðitim Videolarý
```
Giriþ (0-30s) ? Kavramlar (30-120s) ? Uygulama (120-300s) ? Özet (300-360s)
```

### 2. Film Analizi
```
Opening Credits ? Act 1 ? Act 2 ? Climax ? Resolution ? End Credits
```

### 3. Sunum Videolarý
```
Baþlýk ? Problem ? Çözüm ? Demo ? Sonuç ? Sorular
```

## ?? Ýlgili Bileþenler

- **CustomVideoPlayer.razor** - Video oynatýcý
- **TranscriptView.razor** - Transkript görüntüleyici
- **VideoResult.razor** - Analiz sonuç sayfasý

## ?? Lisans

MIT License - Projede özgürce kullanabilirsiniz.

---

**Not**: Bu bileþen, video analiz sistemlerinde sahneleri görsel olarak temsil etmek için ideal bir çözümdür. CustomVideoPlayer ve TranscriptView ile birlikte kullanýldýðýnda tam bir video navigasyon deneyimi sunar.
