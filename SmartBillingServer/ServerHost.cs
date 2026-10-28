using DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer
{
    /// <summary>
    /// How the server is hosted. The standalone server (Program.Main) uses the defaults;
    /// the desktop app runs the same server inside its own process with these set.
    /// </summary>
    public class ServerHostOptions
    {
        public string[] Args { get; init; } = [];
        // Folder used as the content root; relative paths such as "Data Source=SmartBilling.db" resolve against it.
        public string? ContentRootPath { get; init; }
        // Full path of the SQLite file. When null, ConnectionStrings:DefaultConnection is used.
        public string? DatabasePath { get; init; }
        // Address(es) to listen on, for example "http://127.0.0.1:0". When null, the usual ASP.NET Core settings apply.
        public string? Urls { get; init; }
        // Serves the React app from here instead of wwwroot on disk.
        public IFileProvider? WebRootFileProvider { get; init; }
        // Redirects HTTP to HTTPS, but only when the server also listens on an https:// address.
        public bool UseHttpsRedirection { get; init; } = true;
    }

    public static class ServerHost
    {
        private const string DefaultConnectionString = "Data Source=SmartBilling.db";
        // The React dev server (npm start). Other origins can be allowed with Cors:Origins in appsettings.
        private static readonly string[] DevelopmentOrigins = ["http://localhost:3000"];

        public static WebApplication Build(ServerHostOptions options)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = options.Args,
                ContentRootPath = options.ContentRootPath,
                // Controllers live in this assembly, which isn't the entry assembly when the desktop app hosts the server.
                ApplicationName = typeof(ServerHost).Assembly.GetName().Name,
            });

            if (options.Urls != null)
            {
                builder.WebHost.UseUrls(options.Urls);
            }

            var connectionString = options.DatabasePath != null
                ? $"Data Source={options.DatabasePath}"
                : builder.Configuration.GetConnectionString("DefaultConnection") ?? DefaultConnectionString;

            builder.Services.AddControllers().AddApplicationPart(typeof(ServerHost).Assembly);
            builder.Services.AddProblemDetails();
            if (builder.Environment.IsDevelopment())
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));

            // Sign-in (see AuthController). The cookie lasts until the app or browser is closed.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.Cookie.Name = "SmartBilling.Auth";
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SameSite = SameSiteMode.Strict;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    o.ExpireTimeSpan = TimeSpan.FromHours(12);
                    o.SlidingExpiration = true;
                    // This is an API: answer 401/403 instead of redirecting to a login page.
                    // Ends sessions of deleted accounts and changed passwords; picks up changed admin rights.
                    o.Events.OnValidatePrincipal = Accounts.ValidatePrincipalAsync;
                    o.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    o.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });
            // Every endpoint needs a signed-in user unless it is marked [AllowAnonymous].
            builder.Services.AddAuthorization(o =>
            {
                o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

            // The UI is normally served by this server (same origin), so cross-origin calls are only
            // allowed from the React dev server, or from origins listed in Cors:Origins.
            var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                ?? (builder.Environment.IsDevelopment() ? DevelopmentOrigins : []);
            builder.Services.AddCors(o =>
            {
                o.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition");
                });
            });

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IBillRepository, BillRepository>();
            builder.Services.AddScoped<IBarcodeRepository, BarcodeRepository>();
            builder.Services.AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>();
            builder.Services.AddScoped<ILabelDesignRepository, LabelDesignRepository>();

            var listensOnHttps = (options.Urls ?? builder.Configuration["urls"] ?? "").Contains("https://", StringComparison.OrdinalIgnoreCase);

            var app = builder.Build();

            if (options.WebRootFileProvider != null)
            {
                app.Environment.WebRootFileProvider = options.WebRootFileProvider;
            }

            // Bring the database schema up to date (creates the database file on first run).
            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // A plain 500 problem response: no code, paths or stack traces.
                app.UseExceptionHandler();
            }

            if (options.UseHttpsRedirection && listensOnHttps)
            {
                app.UseHttpsRedirection();
            }

            #region For running react UI in the while running the backend API

            app.UseStaticFiles();                // Serve static files from wwwroot

            #endregion

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            // Support React routing. The page itself is public; it shows the sign-in screen.
            app.MapFallbackToFile("index.html").AllowAnonymous();

            return app;
        }
    }
}
