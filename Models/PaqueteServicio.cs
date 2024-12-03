using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class PaqueteServicio
{
    public int IdPaqueteServicio { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdServicio { get; set; }

    public decimal? Costo { get; set; }

    public virtual Paquete? IdPaqueteNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }
}
