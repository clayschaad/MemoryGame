using System.Text.Json;
using Microsoft.JSInterop;
using MemoryGame.Models;

namespace MemoryGame.Services;

public sealed class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private const string StatisticsKey = "memoryGameStatistics";
    
    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<GameStatistics?> LoadStatisticsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StatisticsKey);
            if (string.IsNullOrEmpty(json))
                return null;
            
            return JsonSerializer.Deserialize<GameStatistics>(json);
        }
        catch
        {
            return null;
        }
    }
    
    public async Task SaveStatisticsAsync(GameStatistics statistics)
    {
        var json = JsonSerializer.Serialize(statistics);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StatisticsKey, json);
    }
}
