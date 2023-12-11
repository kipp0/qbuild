
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

using static DbConnect;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();



app.MapPost("/", () => "The server is running!").WithName("PostRoot").WithOpenApi();
app.MapGet("/treeData", () => {
    var db = new DbConnect(app.Configuration);
    // return a list of Part objects
    return db.GetTreeData();
}).WithName("GetTreeData").WithOpenApi();
app.Run();


object[] parts = new object[]
{
    new Part(1,"?", 2, "VALVE",  "Type 1", "?", "00001-254878", "VALVE ASSEMBLY", "?", new List<Part>
    {
        new Part(2,"VALVE", 2, "BODY",  "Type 1", "Item 1", "Part Number 1", "Title 1", "Material 1", new List<Part>
        {
            new Part(3,"BODY", 1, "BODY_SPOOL",  "Type 2", "Item 2", "Part Number 2", "Title 2", "Material 2", null),
            new Part(4,"BODY", 3, "TOP_CONE",  "Type 3", "Item 3", "Part Number 3", "Title 3", "Material 3", null),
        }),
        new Part(5,"VALVE", 4, "ORIFICE", "Type 4", "Item 4", "Part Number 4", "Title 4", "Material 4", new List<Part>
        {
            new Part(6,"ORIFICE", 1, "ORIFICE_PL",  "Type 2", "Item 2", "Part Number 2", "Title 2", "Material 2", null),
            new Part(7,"ORIFICE", 3, "OPLT_RETAINER_BOT",  "Type 3", "Item 3", "Part Number 3", "Title 3", "Material 3", null),
        }),
        new Part(8,"VALVE", 4, "ORIFICE_GASKET",  "Type 4", "Item 4", "Part Number 4", "Title 4", "Material 4", null),
    }),
};

record BomRecord(string parentName, UInt16 quantity, string componentName);
record PartRecord(string name, string type, string item, string partNumber, string title, string material);
public record Part(int id, string parentName, UInt16 quantity, string componentName, string type, string item, string partNumber, string title, string material, List<Part> children);

