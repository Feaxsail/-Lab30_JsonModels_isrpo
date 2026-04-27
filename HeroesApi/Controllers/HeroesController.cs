using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using HeroesApi.Models;
using HeroesApi.Data;

namespace HeroesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase
{
    // Задание 1
    [HttpGet]
    public ActionResult<List<Hero>> GetAll([FromQuery] string? universe = null)
    {
        var heroes = HeroesStore.Heroes.AsEnumerable();
        
        if (!string.IsNullOrEmpty(universe))
        {
            if (Enum.TryParse<Universe>(universe, true, out var universeEnum))
            {
                heroes = heroes.Where(h => h.Universe == universeEnum);
            }
        }
        
        return Ok(heroes.ToList());
    }

    
    [HttpGet("{id}")]
    public ActionResult<Hero> GetById(int id)
    {
        var hero = HeroesStore.Heroes.FirstOrDefault(h => h.Id == id);
        if (hero is null)
        {
            return NotFound(new { message = $"Герой с id={id} не найден" });
        }
        return Ok(hero);
    }

    // Задание 2
    [HttpGet("search")]
    public ActionResult<List<Hero>> Search([FromQuery] string name)
    {
        var heroes = HeroesStore.Heroes
            .Where(h => h.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        return Ok(heroes);
    }

    
    [HttpGet("demo")]
    public ActionResult GetDemo()
    {
        var hero = HeroesStore.Heroes.First();

        var defaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var ourOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return Ok(new
        {
            withDefaultSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, defaultOptions), defaultOptions),
            withOurSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, ourOptions), ourOptions),
            note = "Сравните имена полей и значение universe в двух вариантах"
        });
    }

    
    [HttpGet("serialize")]
    public ActionResult GetSerializeDemo()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var hero = new Hero
        {
            Id = 999,
            Name = "Тестовый герой",
            RealName = "Тест Тестович",
            Universe = Universe.Marvel,
            PowerLevel = 100,
            Powers = new List<string> { "тестирование", "отладка" },
            Weapon = new Weapon { Name = "Клава", IsRanged = false },
            InternalNotes = "Эта заметка не попадёт в JSON"
        };

        string serialized = JsonSerializer.Serialize(hero, options);
        Hero deserialized = JsonSerializer.Deserialize<Hero>(serialized, options);

        return Ok(new
        {
            serializedJson = serialized,
            deserializedObject = deserialized,
            internalNotesAfterDeserialize = deserialized.InternalNotes
        });
    }
}