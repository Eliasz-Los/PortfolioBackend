using System.Text.Json;
using BL.hospital.dto;
using Microsoft.Extensions.Caching.Distributed;

namespace BL.hospital.Caching;

public interface IPatientSearchCache
{
  Task<IReadOnlyList<PatientDto>?> TryGet(string? term, CancellationToken ct = default);
  Task Set(string? term, IReadOnlyList<PatientDto> patients, TimeSpan ttl, CancellationToken ct = default);
}

public class PatientSearchCache : IPatientSearchCache
{
  private static readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);
  private readonly IDistributedCache _cache;
  
  public PatientSearchCache(IDistributedCache cache)
  {
    _cache = cache;
  }
  
  public async Task<IReadOnlyList<PatientDto>?> TryGet(string? term, CancellationToken ct = default)
  {
    var key = BuildKey(term);
    if (key is null) return null;
    
    var cached =  await _cache.GetStringAsync(key, ct);
    if (string.IsNullOrWhiteSpace(cached)) return null; 
    return JsonSerializer.Deserialize<IReadOnlyList<PatientDto>>(cached, jsonOptions);
  }


  public async Task Set(string? term, IReadOnlyList<PatientDto> patients, TimeSpan ttl, CancellationToken ct = default)
  {
    var key = BuildKey(term);
    if (key is null) return;
    var payload = JsonSerializer.Serialize(patients, jsonOptions);
    var options = new DistributedCacheEntryOptions
    {      
      AbsoluteExpirationRelativeToNow = ttl
    };
    await _cache.SetStringAsync(key, payload, options, ct);
  }
  
  private static string? BuildKey(string? term)
  {
    if (string.IsNullOrWhiteSpace(term)) return null;
    
    var normalizedTerm = string.Join(' ',
      term
        .Trim()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .ToLowerInvariant();
      
    return $"patient-search:v1:{normalizedTerm}";
  }
}