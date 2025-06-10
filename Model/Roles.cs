namespace backend_serfi.Model
{
    public class Roles
    {

        public long Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Permiso> Permisos { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }


    }
}
