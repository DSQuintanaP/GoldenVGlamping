using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Mueble
{
    public int IdMueble { get; set; }

    public string? NombreMueble { get; set; }

    public bool? Estado { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
}
