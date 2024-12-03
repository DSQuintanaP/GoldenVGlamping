using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int? IdHabitacion { get; set; }

    public int? IdMueble { get; set; }

    public virtual Habitacione? IdHabitacionNavigation { get; set; }

    public virtual Mueble? IdMuebleNavigation { get; set; }
}
