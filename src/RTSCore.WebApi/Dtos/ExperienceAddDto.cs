using System.ComponentModel.DataAnnotations;

namespace RTSCore.WebApi.Dtos;

public readonly record struct ExperienceAddDto
(
    [Range(0, 5000, ErrorMessage = "За раз можно начислить от 0 до 5000 опыта")]
    int Amount
);