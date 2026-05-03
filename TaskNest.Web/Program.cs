using TaskNest.API.Extensions;
using TaskNest.API.Notifi;

var builder = WebApplication.CreateBuilder(args);


// Allow larger files
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024;
    options.ValueLengthLimit = int.MaxValue;            
    options.MemoryBufferThreshold = int.MaxValue;      
});

builder.Services
    .AddAppControllersAndSwagger()
    .AddAppCors()
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddAppAuthentication(builder.Configuration)
    .AddAuthorization()
    .AddAppServices()
    .AddAppMappings()
    .AddAppSignalR();

builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        await DbInitializer.SeedAsync(db);
        await IdentitySeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
app.Run();