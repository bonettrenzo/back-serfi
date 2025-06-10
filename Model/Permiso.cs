namespace backend_serfi.Model
{
    public class Permiso
    {
        public long Id { get; set; }
        public String NombrePermiso { get; set; }
        public ICollection<Roles> Roles { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }

    }
}
