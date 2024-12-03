using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Abono
{
    public int IdAbono { get; set; }

    public int? IdReserva { get; set; }

    public DateTime? FechaAbono { get; set; }

    public decimal? CantAbono { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }

    public virtual ICollection<ImagenAbono> ImagenAbonos { get; set; } = new List<ImagenAbono>();
}
