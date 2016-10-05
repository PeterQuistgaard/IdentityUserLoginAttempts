# IdentityUserLoginAttempts
Add tracking of user login attempts to ASP.Net Identity 2.2.1


## Resume
- Add new class LoginAttempt to IdentityModels.cs
- Modify class ApplicationUser in IdentityModels 
- Override PasswordSignInAsync in ApplicationSignInManager (in IdentityCongig.cs)

## How to create the solution

Create a new project. Use ASP.NET WebApplication (.NET Framework)


![Image01](image1.png)

![Image01](image2.png)

## Make some changes in the generated code.

All changes are placed between #region Change and #endregion Change.


### Web.Config
Change connectionStrings to match your prefered database. 
     
```XML

  <connectionStrings>
   <!-- #region Change -->
    <add name="DefaultConnection" 
         providerName="System.Data.SqlClient" 
         connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=IdentityUserLoginAttempt;Integrated Security=SSPI" />
   <!-- #endregion Change-->      
  </connectionStrings>

```

### Add new class LoginAttempt to IdentityModels.cs
```#C
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
```

### Modify class ApplicationUser in IdentityModels 
```#C
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
```

### Override PasswordSignInAsync in ApplicationSignInManager (in IdentityCongig.cs)
```#c
#region Change
public async override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
{
    var result = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);

    ///Find user by name. If user exist - then add loginAttempt to the user
    var user = await UserManager.FindByNameAsync(userName);
    if (user != null)
    {
        //Get users ipAddress
        string ip =HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(ip)) ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        user.LoginAttempts.Add(new LoginAttempt { UserId = user.Id, SignInStatus = result });
        await UserManager.UpdateAsync(user);
    }

    return result;
}
#endregion Change  
```

### AccountController Login Action
To enable password failures to trigger account lockout, change to shouldLockout: true

```#C
#region Change
// var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
#endregion
```



### Action LoginAttempts
Add new Action to Controllers\ManagerController

```#c
#region Change
// GET: /Manage/LoginAttempts
public async Task<ActionResult> LoginAttempts()
{
    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
    return View(user.LoginAttempts.OrderByDescending(l=>l.LoginAttemptDateUtc).ToList());           
}
#endregion
```

### View LoginAttempts 
Add new View to Views\Manage


```cshtml
<!-- #region Change -->
@model IEnumerable<IdentityUserLoginAttempts.Models.LoginAttempt>

@{
    ViewBag.Title = "LoginAttempts";
}

<h2>LoginAttempts</h2>

<table class="table">
    <tr>
        <th>UserName</th>
        <th>LoginAttemptDate</th>
        <th>IpAddress</th>
        <th>SignInStatus</th>
    </tr>

@foreach (var item in Model) {
    <tr>
        <td>
            @item.User.UserName
        </td>
        <td>
            @item.IpAddress
        </td>
        <td>
            @item.LoginAttemptDateUtc.ToLocalTime()
        </td>
        <td>
            @item.SignInStatus
        </td>
    </tr>
}
</table>
<!-- #endregion Change -->
```

### Modify Views\Manage\Index
Insert a link to LoginAttempts.

```cshtml
<!-- #region Change -->        
<dt>
    LoginAttempts:
</dt>
<dd>@Html.ActionLink("LoginAttempts", "LoginAttempts")</dd>
<!-- #endregion Change -->
```