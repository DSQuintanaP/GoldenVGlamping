using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Habitacione
{
    public int IdHabitacion { get; set; }

    public int? IdTipoHabitacion { get; set; }

    public string? NombreHabitacion { get; set; }

    public bool? Estado { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Costo { get; set; }

    public virtual TipoHabitacione? IdTipoHabitacionNavigation { get; set; }

    public virtual ICollection<ImagenHabitacion> ImagenHabitacions { get; set; } = new List<ImagenHabitacion>();

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();

    public virtual ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
}
