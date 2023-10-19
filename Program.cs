using FileRenamer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/proposeChanges", async (IFileRenamingService fileRenamingService, RenamingTask task) =>
{
    return await fileRenamingService.ProposeChangesAsync(task);
});

app.MapPost("/executeRenaming", async (IFileRenamingService fileRenamingService, List<ConfirmedChange> confirmedChanges) =>
{
    return await fileRenamingService.ExecuteRenamingAsync(confirmedChanges);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
