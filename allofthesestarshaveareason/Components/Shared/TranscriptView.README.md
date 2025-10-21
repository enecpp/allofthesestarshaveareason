# TranscriptView Component

## ?? Genel Bak��

`TranscriptView.razor` video transkript segmentlerini g�r�nt�lemek i�in tasarlanm�� yeniden kullan�labilir bir Blazor bile�enidir. Kullan�c� bir transkript segmentine t�klad���nda, ilgili video zaman�na atlamak i�in event callback tetikler.

## ? �zellikler

- ? **MudBlazor Integration**: Modern ve responsive tasar�m
- ? **Event Callbacks**: Segment t�klama olaylar�n� �st bile�enlere iletir
- ? **Zaman Formatlama**: Saniye de�erlerini `mm:ss` format�nda g�sterir
- ? **Null Safety**: Bo� segment listelerini g�venli �ekilde y�netir
- ? **Scroll Support**: Uzun transkriptler i�in kayd�rma deste�i
- ? **Hover Effects**: Kullan�c� deneyimi i�in g�rsel geri bildirim
- ? **Responsive Design**: Mobil ve masa�st� cihazlara uyumlu

## ?? Kurulum

Dosya zaten projede mevcut:
```
allofthesestarshaveareason/Components/Shared/TranscriptView.razor
allofthesestarshaveareason/Components/Shared/TranscriptView.razor.css
```

## ?? Kullan�m

### Temel Kullan�m

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
            Speaker = "Konu�mac� 1", 
            Text = "Merhaba, bug�n makine ��renmesi hakk�nda konu�aca��z.", 
            StartTime = 0, 
            EndTime = 5.5 
        },
        new TranscriptSegmentDto 
        { 
            Id = 2,
            Speaker = "Konu�mac� 2", 
            Text = "Harika, ben de bu konuya �ok merakl�y�m.", 
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

### Arama Sonu�lar� ile Kullan�m

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
        Arama Sonu�lar� (@searchResults.Count)
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
| `Segments` | `IEnumerable<TranscriptSegmentDto>?` | No | `null` | G�r�nt�lenecek transkript segmentleri |
| `OnSegmentClicked` | `EventCallback<double>` | No | - | Segment t�kland���nda tetiklenir, ba�lang�� zaman�n� (saniye) d�nd�r�r |

### TranscriptSegmentDto Yap�s�

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
| `SegmentClicked` | `Task` | `double time` | Private metod - segment t�klamas�n� i�ler |
| `FormatTime` | `string` | `double seconds` | Private metod - saniyeyi `mm:ss` format�na �evirir |

## ?? CSS Customization

`TranscriptView.razor.css` dosyas�n� d�zenleyerek g�r�n�m� �zelle�tirebilirsiniz:

```css
/* Maksimum y�ksekli�i de�i�tir */
.transcript-view-container {
    max-height: 800px; /* Varsay�lan: 600px */
}

/* Hover rengini �zelle�tir */
.transcript-segment:hover {
    background-color: rgba(33, 150, 243, 0.1); /* Mavi ton */
}

/* Padding de�erlerini ayarla */
.transcript-segment {
    padding: 20px; /* Varsay�lan: 16px */
}
```

## ?? Tam Video Analiz Sistemi �rne�i

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
                        Arama Sonu�lar� (@searchResults.Count)
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

## ?? �zelle�tirme �rnekleri

### Segment'lere Vurgu Ekleme

Aktif video zaman�na g�re segment'leri vurgulayabilirsiniz:

```razor
<TranscriptView Segments="@GetHighlightedSegments()" 
                OnSegmentClicked="JumpToSegment" />

@code {
    private double currentVideoTime;

    private IEnumerable<TranscriptSegmentDto> GetHighlightedSegments()
    {
        return transcriptSegments?.Select(s => 
        {
            // Mevcut segment'i i�aretle
            if (s.StartTime <= currentVideoTime && s.EndTime >= currentVideoTime)
            {
                // �zel CSS class veya styling ekleyebilirsiniz
            }
            return s;
        }) ?? Enumerable.Empty<TranscriptSegmentDto>();
    }
}
```

### Sayfalama Ekleme

Uzun transkriptler i�in sayfalama:

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

## ?? �nemli Notlar

1. **Null Safety**: Segments parametresi null olabilir, bu durumda bilgilendirme mesaj� g�sterilir.

2. **Performance**: �ok uzun transkriptler i�in sanal scrolling veya sayfalama kullanmay� d���n�n.

3. **Event Callback**: `OnSegmentClicked` callback'i optional'd�r. Tan�mlanmam��sa t�klama olay� yok say�l�r.

4. **Time Format**: Zaman format� `mm:ss` �eklindedir. Saat gerekiyorsa `FormatTime` metodunu g�ncelleyin:
   ```csharp
   private string FormatTime(double seconds)
   {
       var timeSpan = TimeSpan.FromSeconds(seconds);
       return timeSpan.ToString(@"hh\:mm\:ss"); // Saat:Dakika:Saniye
   }
   ```

## ?? Troubleshooting

### Segments g�r�nm�yor

```csharp
// Segments'in null veya bo� olmad���ndan emin olun
@code {
    protected override void OnInitialized()
    {
        Console.WriteLine($"Segments count: {Segments?.Count() ?? 0}");
    }
}
```

### OnSegmentClicked tetiklenmiyor

```csharp
// EventCallback'in tan�ml� oldu�undan emin olun
<TranscriptView Segments="@segments" 
                OnSegmentClicked="HandleClick" />  <!-- Method ad� do�ru mu? -->

@code {
    private async Task HandleClick(double time)
    {
        Console.WriteLine($"Clicked at: {time}");
    }
}
```

## ?? Lisans

MIT License - Projede �zg�rce kullanabilirsiniz.

---

**Not**: Bu bile�en `CustomVideoPlayer.razor` ile birlikte kullan�lmak �zere tasarlanm��t�r ve video analiz sistemlerinde transkript g�r�nt�leme i�in ideal bir ��z�md�r.
