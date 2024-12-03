using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class ImagenAbono
{
    public int IdImagenAbono { get; set; }

    public int? IdImagen { get; set; }

    public int? IdAbono { get; set; }

    public virtual Abono? IdAbonoNavigation { get; set; }

    public virtual Imagene? IdImagenNavigation { get; set; }
}
