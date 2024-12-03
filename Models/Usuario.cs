using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? NroDocumento { get; set; }

    public int? IdTipoDocumento { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public string? Contrasena { get; set; }

    public bool? Estado { get; set; }

    public int? IdRol { get; set; }

    public virtual Role? IdRolNavigation { get; set; }

    public virtual TipoDocumento? IdTipoDocumentoNavigation { get; set; }
}
