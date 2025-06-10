namespace backend_serfi.Model
{
    public class RolPermiso
    {
        public long Id { get; set; }

        public long RolId { get; set; }
        public Roles Rol { get; set; }

        public long PermisoId { get; set; }
        public Permiso Permiso { get; set; }

    }
}
