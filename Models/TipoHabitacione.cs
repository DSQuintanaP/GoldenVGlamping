using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class TipoHabitacione
{
    public int IdTipoHabitacion { get; set; }

    public string? NombreTipoHabitacion { get; set; }

    public int? NumeroPersonas { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>();
}
