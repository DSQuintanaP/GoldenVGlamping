using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class TipoDocumento
{
    public int IdTipoDocumento { get; set; }

    public string? NomTipoDocumento { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
