using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;

namespace IdentityUserLoginAttempts.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        #region Change
        public ApplicationUser() : base()
        {
            LoginAttempts = new List<LoginAttempt>();
        }
        public virtual IList<LoginAttempt> LoginAttempts { get; set; }

        #endregion Change



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
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
    }

    #region Change
    [Table("AspNetUserLoginAttempts")]
    public class LoginAttempt
    {
        //Constructor
        public LoginAttempt()
        {
            LoginAttemptDateUtc = DateTime.UtcNow;
        }

        [Key, Column(Order = 0)]
        public DateTime LoginAttemptDateUtc { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        public SignInStatus SignInStatus { get; set; }

        public string IpAddress { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
    #endregion Change


}