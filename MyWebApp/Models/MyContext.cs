using Microsoft.EntityFrameworkCore;

namespace MyWebApp.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }
    }
}
