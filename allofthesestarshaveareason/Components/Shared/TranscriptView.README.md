# TranscriptView Component

## ?? Genel Bakýþ

`TranscriptView.razor` video transkript segmentlerini görüntülemek için tasarlanmýþ yeniden kullanýlabilir bir Blazor bileþenidir. Kullanýcý bir transkript segmentine týkladýðýnda, ilgili video zamanýna atlamak için event callback tetikler.

## ? Özellikler

- ? **MudBlazor Integration**: Modern ve responsive tasarým
- ? **Event Callbacks**: Segment týklama olaylarýný üst bileþenlere iletir
- ? **Zaman Formatlama**: Saniye deðerlerini `mm:ss` formatýnda gösterir
- ? **Null Safety**: Boþ segment listelerini güvenli þekilde yönetir
- ? **Scroll Support**: Uzun transkriptler için kaydýrma desteði
- ? **Hover Effects**: Kullanýcý deneyimi için görsel geri bildirim
- ? **Responsive Design**: Mobil ve masaüstü cihazlara uyumlu

## ?? Kurulum

Dosya zaten projede mevcut:
```
allofthesestarshaveareason/Components/Shared/TranscriptView.razor
allofthesestarshaveareason/Components/Shared/TranscriptView.razor.css
```

## ?? Kullaným

### Temel Kullaným

```razor
@page "/transcript-demo"

<TranscriptView Segments="@transcriptSegments" 
                OnSegmentClicked="HandleSegmentClick" />

@code {
    private List<TranscriptSegmentDto> transcriptSegments = new()
    {
        new TranscriptSegmentDto 
        { 
            Id = 1,
            Speaker = "Konuþmacý 1", 
            Text = "Merhaba, bugün makine öðrenmesi hakkýnda konuþacaðýz.", 
            StartTime = 0, 
            EndTime = 5.5 
        },
        new TranscriptSegmentDto 
        { 
            Id = 2,
            Speaker = "Konuþmacý 2", 
            Text = "Harika, ben de bu konuya çok meraklýyým.", 
            StartTime = 5.5, 
            EndTime = 10.2 
        }
    };

    private void HandleSegmentClick(double time)
    {
        Console.WriteLine($"User clicked segment at time: {time}s");
    }
}
```

### CustomVideoPlayer ile Entegrasyon

```razor
@page "/video-with-transcript"

<MudGrid>
    <MudItem xs="12" md="8">
        <CustomVideoPlayer @ref="videoPlayer" VideoUrl="@videoUrl" />
    </MudItem>
    
    <MudItem xs="12" md="4">
        <MudPaper Class="pa-4">
            <MudText Typo="Typo.h6" Class="mb-4">Transkript</MudText>
            <TranscriptView Segments="@transcriptSegments" 
                          OnSegmentClicked="JumpToTime" />
        </MudPaper>
    </MudItem>
</MudGrid>

@code {
    private CustomVideoPlayer? videoPlayer;
    private string videoUrl = "https://example.com/video.mp4";
    private List<TranscriptSegmentDto>? transcriptSegments;

    protected override async Task OnInitializedAsync()
    {
        // Load transcript from API
        transcriptSegments = await LoadTranscriptFromApi();
    }

    private async Task JumpToTime(double startTime)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(startTime);
            await videoPlayer.Play();
        }
    }

    private async Task<List<TranscriptSegmentDto>> LoadTranscriptFromApi()
    {
        // API call implementation
        return new List<TranscriptSegmentDto>();
    }
}
```

### Arama Sonuçlarý ile Kullaným

```razor
@page "/search-results"
@inject HttpClient Http

<MudTextField @bind-Value="searchQuery" 
              Label="Videoda Ara" 
              Variant="Variant.Outlined"
              OnAdornmentClick="SearchInVideo"
              Adornment="Adornment.End"
              AdornmentIcon="@Icons.Material.Filled.Search" />

@if (searchResults != null)
{
    <MudText Typo="Typo.h6" Class="mt-4">
        Arama Sonuçlarý (@searchResults.Count)
    </MudText>
    <TranscriptView Segments="@searchResults" 
                    OnSegmentClicked="JumpToSearchResult" />
}

@code {
    private string searchQuery = "";
    private List<TranscriptSegmentDto>? searchResults;
    private CustomVideoPlayer? videoPlayer;

    private async Task SearchInVideo()
    {
        if (string.IsNullOrWhiteSpace(searchQuery)) return;

        searchResults = await Http.GetFromJsonAsync<List<TranscriptSegmentDto>>(
            $"api/VideoAnalysis/result/123/search?query={Uri.EscapeDataString(searchQuery)}");
    }

    private async Task JumpToSearchResult(double time)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(time);
            await videoPlayer.Play();
        }
    }
}
```

## ?? API Reference

### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `Segments` | `IEnumerable<TranscriptSegmentDto>?` | No | `null` | Görüntülenecek transkript segmentleri |
| `OnSegmentClicked` | `EventCallback<double>` | No | - | Segment týklandýðýnda tetiklenir, baþlangýç zamanýný (saniye) döndürür |

### TranscriptSegmentDto Yapýsý

```csharp
public class TranscriptSegmentDto
{
    public int Id { get; set; }
    public string Speaker { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }
}
```

### Methods

| Method | Return Type | Parameters | Description |
|--------|-------------|------------|-------------|
| `SegmentClicked` | `Task` | `double time` | Private metod - segment týklamasýný iþler |
| `FormatTime` | `string` | `double seconds` | Private metod - saniyeyi `mm:ss` formatýna çevirir |

## ?? CSS Customization

`TranscriptView.razor.css` dosyasýný düzenleyerek görünümü özelleþtirebilirsiniz:

```css
/* Maksimum yüksekliði deðiþtir */
.transcript-view-container {
    max-height: 800px; /* Varsayýlan: 600px */
}

/* Hover rengini özelleþtir */
.transcript-segment:hover {
    background-color: rgba(33, 150, 243, 0.1); /* Mavi ton */
}

/* Padding deðerlerini ayarla */
.transcript-segment {
    padding: 20px; /* Varsayýlan: 16px */
}
```

## ?? Tam Video Analiz Sistemi Örneði

```razor
@page "/analysis/{ResultId:int}"
@using allofthosestarshaveareason.DTOs
@inject HttpClient Http

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-4">
    <MudGrid>
        <!-- Video Player -->
        <MudItem xs="12" lg="8">
            <MudPaper Class="pa-4">
                <CustomVideoPlayer @ref="videoPlayer" 
                                 VideoUrl="@result?.VideoUrl"
                                 OnTimeChanged="OnVideoTimeChanged" />
            </MudPaper>
        </MudItem>

        <!-- Transkript ve Arama -->
        <MudItem xs="12" lg="4">
            <!-- Arama -->
            <MudPaper Class="pa-4 mb-4">
                <MudTextField @bind-Value="searchQuery" 
                            Label="Videoda Ara" 
                            Variant="Variant.Outlined"
                            OnAdornmentClick="SearchInVideo"
                            Adornment="Adornment.End"
                            AdornmentIcon="@Icons.Material.Filled.Search" />
                
                @if (searchResults != null && searchResults.Any())
                {
                    <MudText Typo="Typo.h6" Class="mt-4 mb-2">
                        Arama Sonuçlarý (@searchResults.Count)
                    </MudText>
                    <TranscriptView Segments="@searchResults" 
                                  OnSegmentClicked="JumpToSegment" />
                }
            </MudPaper>

            <!-- Tam Transkript -->
            <MudPaper Class="pa-4">
                <MudText Typo="Typo.h6" Class="mb-2">Transkript</MudText>
                <TranscriptView Segments="@result?.Transcript" 
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
    private List<TranscriptSegmentDto>? searchResults;
    private string searchQuery = "";
    private double currentTime;

    protected override async Task OnInitializedAsync()
    {
        result = await Http.GetFromJsonAsync<VideoAnalysisResultDto>(
            $"api/VideoAnalysis/result/{ResultId}");
    }

    private async Task SearchInVideo()
    {
        if (string.IsNullOrWhiteSpace(searchQuery)) return;

        searchResults = await Http.GetFromJsonAsync<List<TranscriptSegmentDto>>(
            $"api/VideoAnalysis/result/{ResultId}/search?query={Uri.EscapeDataString(searchQuery)}");
    }

    private async Task JumpToSegment(double startTime)
    {
        if (videoPlayer != null)
        {
            await videoPlayer.SeekTo(startTime);
            await videoPlayer.Play();
        }
    }

    private void OnVideoTimeChanged(double time)
    {
        currentTime = time;
        StateHasChanged();
    }
}
```

## ?? Özelleþtirme Örnekleri

### Segment'lere Vurgu Ekleme

Aktif video zamanýna göre segment'leri vurgulayabilirsiniz:

```razor
<TranscriptView Segments="@GetHighlightedSegments()" 
                OnSegmentClicked="JumpToSegment" />

@code {
    private double currentVideoTime;

    private IEnumerable<TranscriptSegmentDto> GetHighlightedSegments()
    {
        return transcriptSegments?.Select(s => 
        {
            // Mevcut segment'i iþaretle
            if (s.StartTime <= currentVideoTime && s.EndTime >= currentVideoTime)
            {
                // Özel CSS class veya styling ekleyebilirsiniz
            }
            return s;
        }) ?? Enumerable.Empty<TranscriptSegmentDto>();
    }
}
```

### Sayfalama Ekleme

Uzun transkriptler için sayfalama:

```razor
<TranscriptView Segments="@GetPagedSegments()" 
                OnSegmentClicked="JumpToSegment" />

<MudPagination Count="@totalPages" 
               SelectedChanged="OnPageChanged" />

@code {
    private int currentPage = 1;
    private int pageSize = 20;
    private int totalPages => (int)Math.Ceiling((transcriptSegments?.Count() ?? 0) / (double)pageSize);

    private IEnumerable<TranscriptSegmentDto> GetPagedSegments()
    {
        return transcriptSegments?
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize) 
            ?? Enumerable.Empty<TranscriptSegmentDto>();
    }

    private void OnPageChanged(int page)
    {
        currentPage = page;
    }
}
```

## ?? Önemli Notlar

1. **Null Safety**: Segments parametresi null olabilir, bu durumda bilgilendirme mesajý gösterilir.

2. **Performance**: Çok uzun transkriptler için sanal scrolling veya sayfalama kullanmayý düþünün.

3. **Event Callback**: `OnSegmentClicked` callback'i optional'dýr. Tanýmlanmamýþsa týklama olayý yok sayýlýr.

4. **Time Format**: Zaman formatý `mm:ss` þeklindedir. Saat gerekiyorsa `FormatTime` metodunu güncelleyin:
   ```csharp
   private string FormatTime(double seconds)
   {
       var timeSpan = TimeSpan.FromSeconds(seconds);
       return timeSpan.ToString(@"hh\:mm\:ss"); // Saat:Dakika:Saniye
   }
   ```

## ?? Troubleshooting

### Segments görünmüyor

```csharp
// Segments'in null veya boþ olmadýðýndan emin olun
@code {
    protected override void OnInitialized()
    {
        Console.WriteLine($"Segments count: {Segments?.Count() ?? 0}");
    }
}
```

### OnSegmentClicked tetiklenmiyor

```csharp
// EventCallback'in tanýmlý olduðundan emin olun
<TranscriptView Segments="@segments" 
                OnSegmentClicked="HandleClick" />  <!-- Method adý doðru mu? -->

@code {
    private async Task HandleClick(double time)
    {
        Console.WriteLine($"Clicked at: {time}");
    }
}
```

## ?? Lisans

MIT License - Projede özgürce kullanabilirsiniz.

---

**Not**: Bu bileþen `CustomVideoPlayer.razor` ile birlikte kullanýlmak üzere tasarlanmýþtýr ve video analiz sistemlerinde transkript görüntüleme için ideal bir çözümdür.
