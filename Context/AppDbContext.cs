
using Microsoft.EntityFrameworkCore;
using backend_serfi.Model;

namespace backend_serfi.Context
  {
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }
        public DbSet<User> Users {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, Nombre = "Admin" },
                new Roles { Id = 2, Nombre = "Operador"},
                new Roles { Id = 3, Nombre = "Cliente"}
            );

            modelBuilder.Entity<Permiso>().HasData(
                new Permiso { Id = 1, NombrePermiso = "CrearUsuario" },
                new Permiso { Id = 2, NombrePermiso = "EditarUsuario" },
                new Permiso { Id = 3, NombrePermiso = "EliminarUsuario" },
                new Permiso { Id = 4, NombrePermiso = "LeerUsuarios" },
                new Permiso { Id = 5, NombrePermiso = "LeerPropiosDatos" }
             );

            modelBuilder.Entity<RolPermiso>().HasData(
                // Admin: todos
                new RolPermiso { Id = 1, RolId = 1, PermisoId = 1 },
                new RolPermiso { Id = 2, RolId = 1, PermisoId = 2 },
                new RolPermiso { Id = 3, RolId = 1, PermisoId = 3 },
                new RolPermiso { Id = 4, RolId = 1, PermisoId = 4 },
                new RolPermiso { Id = 5, RolId = 1, PermisoId = 5 },

                // Operador: leer y editar usuarios cliente
                new RolPermiso { Id = 6, RolId = 2, PermisoId = 2 },
                new RolPermiso { Id = 7, RolId = 2, PermisoId = 4 },

                // Cliente: leer propios datos
                new RolPermiso { Id = 8, RolId = 3, PermisoId = 5 }
             );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    NombreCompleto = "Renzo Admin",
                    UltimaConexion = DateTime.SpecifyKind(DateTime.Parse("2025-06-09T00:00:00Z"), DateTimeKind.Utc),
                    Pais = "Colombia",
                    Email = "admin@admin.com",
                    RolesId = 1,
                    Password = "$2a$11$tiPC20PpDzWh.C23tRgG5ucYuo0e2Viy2G7HPTncbBwGKNr2nxU7a"
                }   
             );
        }
        

    }
}
