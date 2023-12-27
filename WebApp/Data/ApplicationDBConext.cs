using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data
{
    public class ApplicationDBConext: IdentityDbContext // instead of dbContext we use IdentityDbContext because it contains all the things that we need
                                                        // like user, role etc. all of the Db sets are defined here
    {
        public ApplicationDBConext(DbContextOptions<ApplicationDBConext> options):base(options)
        {
        }
    }
}


//this constructor is responsible for configuring the database context using the provided options.
//The : base(options) part indicates that it calls the constructor of the base class (IdentityDbContext) with the provided options from program.cs,
//ensuring that the base class is properly initialized.
