using System.ComponentModel.DataAnnotations;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Dtos;

public readonly record struct UnitCreateDto(string Id, UnitType Type, FactionType Faction);