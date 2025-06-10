namespace backend_serfi.Model
{
    public class User
    {
        public long Id { get; set; }
        public String NombreCompleto  { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public DateTime? UltimaConexion   { get; set; }
        public string Pais { get; set; }
        public long RoleId { get; set; }
        //public Roles Roles { get; set; }

    }
}
