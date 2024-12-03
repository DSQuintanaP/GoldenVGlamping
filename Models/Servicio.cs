using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public int? IdTipoServicio { get; set; }

    public string? NomServicio { get; set; }

    public decimal? Costo { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<DetalleReservaServicio> DetalleReservaServicios { get; set; } = new List<DetalleReservaServicio>();

    public virtual TipoServicio? IdTipoServicioNavigation { get; set; }

    public virtual ICollection<ImagenServicio> ImagenServicios { get; set; } = new List<ImagenServicio>();

    public virtual ICollection<PaqueteServicio> PaqueteServicios { get; set; } = new List<PaqueteServicio>();
}
