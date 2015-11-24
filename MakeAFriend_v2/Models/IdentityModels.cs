using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MakeAFriend_v2.Models;

namespace MakeAFriend_v2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string UserStatus { get; set; }      // Online status of user.
        public string ConnectionId { get; set; }    // Connection Id.
        public int NumReports { get; set; }         // Number of reports user has.
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Friends> MyFriends { get; set; }
    }
    
}

public class Friends
{
    [Key]
    [Required]
    [Display(Name = "Id")]
    public string Id { get; set; }
    [ForeignKey("Id")]
    public virtual ApplicationUser User { get; set; }

    [Required]
    [Display(Name = "Friend Id")]
    public string FriendId { get; set; }

    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; }

    [Required]
    [Display(Name = "User Status")]
    public string UserStatus { get; set; }

    [Required]
    [Display(Name = "Connection Id")]
    public string ConnectionId { get; set; }

}