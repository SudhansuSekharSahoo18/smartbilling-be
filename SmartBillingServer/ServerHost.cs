using DataAccess.Repository;
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
        public bool UseHttpsRedirection { get; init; } = true;
    }

    public static class ServerHost
    {
        private const string DefaultConnectionString = "Data Source=SmartBilling.db";

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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));

            builder.Services.AddCors(o =>
            {
                o.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IBillRepository, BillRepository>();
            builder.Services.AddScoped<IBarcodeRepository, BarcodeRepository>();
            builder.Services.AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>();
            builder.Services.AddScoped<ILabelDesignRepository, LabelDesignRepository>();

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

            #region For running react UI in the while running the backend API

            app.UseStaticFiles();                // Serve static files from wwwroot
            app.MapFallbackToFile("index.html"); // Support React routing

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            if (options.UseHttpsRedirection)
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
