using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class PermisosRole
{
    public int IdPermisosRoles { get; set; }

    public int? IdRol { get; set; }

    public int? IdPermiso { get; set; }

    public virtual Permiso? IdPermisoNavigation { get; set; }

    public virtual Role? IdRolNavigation { get; set; }
}
