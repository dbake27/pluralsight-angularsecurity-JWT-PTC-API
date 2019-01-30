using Microsoft.EntityFrameworkCore;

namespace PtcApi.Model
{
  public class PtcDbContext : DbContext
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
     public DbSet<AppUser> Users { get; set; }
      public DbSet<AppUserClaim> Claims { get; set; }

    // private const string CONN =
    //               @"Server=Localhost;
    //                 Database=PTC-Pluralsight;
    //                 Trusted_Connection=True;
    //                 MultipleActiveResultSets=true";

    //this works to sqllite and starting debug for the ptcapi to http://5000/api/products but
    //not postman  prob due to sqllight not installed only referring to the mdb below
    //private const string CONN = @"Server=(localdb)\MSSQLLocalDB;
    //Database=PTC-Pluralsight;
    //AttachDbFilename=C:\Angular6\Pluralsight-AngularSecurity-JWT\SqlData\PTC-Pluralsight.mdf;
    //MultipleActiveResultSets=true";
 

 //connection to sqlserver 2016 on local laptop with ptc-pluralsight db created with objects per the install sql script


private const string CONN =
@"Server=LAPTOP-ACKHS4CB;Database=PTC-Pluralsight;user= LAPTOP-ACKHS4CB\\baker;Trusted_Connection=True;Integrated Security=SSPI";

    protected override void OnConfiguring(
                DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(CONN);
    }
  }
}
