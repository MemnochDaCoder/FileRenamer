using FileRenamer.Interfaces;
using FileRenamer.Models;
using FileRenamer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ITvDbService, TvDbService>();
builder.Services.AddTransient<IFileRenamingService, FileRenamingService>();

var app = builder.Build();

app.MapPost("/proposeChanges", async (IFileRenamingService fileRenamingService, RenamingTask task) =>
{
    return await fileRenamingService.ProposeChangesAsync(task);
});

//app.MapPost("/executeRenaming", async (IFileRenamingService fileRenamingService, List<ConfirmedChangeModel> confirmedChanges) =>
//{
//    return await fileRenamingService.ExecuteRenamingAsync(confirmedChanges);
//});

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
