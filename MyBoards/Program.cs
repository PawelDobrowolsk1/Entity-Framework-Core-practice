using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBoards;
using MyBoards.Dto;
using MyBoards.Entities;
using MyBoards.Migrations;
using MyBoards.Sieve;
using Sieve.Models;
using Sieve.Services;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddDbContext<MyBoardsContext>(
    options => options
    //.UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnetionString"))
    );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("sieve", async ([FromBody] SieveModel query, ISieveProcessor sieveProcessor, MyBoardsContext db) =>
{
    var epics = db.Epics
    .Include(e => e.Author)
    .AsQueryable();

    var dtos = await sieveProcessor
        .Apply(query, epics)
        .Select(e => new EpicDto()
        {
            Id = e.Id,
            Area= e.Area,
            Priority= e.Priority,
            StartDate= e.StartDate,
            AuthorFullName = e.Author.FullName
        })
        .ToListAsync();

    var totalCount = await sieveProcessor
        .Apply(query, epics, applyPagination: false, applySorting: false)
        .CountAsync();

    var result = new PageResult<EpicDto>(dtos, totalCount, query.PageSize.Value, query.Page.Value);

    return result;
});

#region

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

DataGenerator.Seed(dbContext);

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FullName = "User One",
        Address = new Address()
        {
            City = "Warszawa",
            Street = "Szeroka"
        }
    };

    var user2 = new User()
    {
        Email = "user2@test.com",
        FullName = "User Two",
        Address = new Address()
        {
            City = "Krakow",
            Street = "D³uga"
        }
    };

    dbContext.Users.AddRange(user1, user2);
    dbContext.SaveChanges();
}


app.MapGet("pagination", async (MyBoardsContext db) =>
{
    //user input
    var filter = "user";
    string sortBy = "FullName"; // "FullName" "Email" null
    bool sortByDescending = false;
    int pageNumber = 1;
    int pageSize = 10;
    //

    var query = db.Users
    .Where(u => filter == null ||
        u.Email.ToLower().Contains(filter.ToLower()) || u.FullName.ToLower().Contains(filter.ToLower()));

    var totalCount = query.Count();

    if (sortBy != null)
    {
        var columnsSelector = new Dictionary<string, Expression<Func<User, object>>>()
        {
            { nameof(User.Email), user => user.Email },
            { nameof(User.FullName),  user => user.FullName }
        };

        var sortByExpression = columnsSelector[sortBy];
        query = sortByDescending ? query.OrderByDescending(sortByExpression) : query.OrderBy(sortByExpression);

    }

    var result = await query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

    var pagedResult = new PageResult<User>(result, totalCount, pageSize, pageNumber);

    return pagedResult;
});

app.MapGet("data", async (MyBoardsContext db) =>
{
    var userComments = await db.Users
     .Include(u => u.Address)
     .Where(u => u.Address.Country == "Albania")
     .SelectMany(u => u.Comments)
     .Select(c => c.Message)
     .ToListAsync();

    return userComments;
});

app.MapPost("update", async (MyBoardsContext db) =>
{
    Epic epic = await db.Epics.FirstAsync(epic => epic.Id == 1);

    var rejectedState = await db.WorkItemsStates.FirstAsync(a => a.Value == "Rejected");

    epic.State = rejectedState;

    await db.SaveChangesAsync();

    return epic;
});

app.MapPost("create", async (MyBoardsContext db) =>
{
    var address = new Address()
    {
        Id = Guid.Parse("b323dd7c-776a-4cf6-a92a-12df154b4a2c"),
        City = "Krakow",
        Country = "Poland",
        Street = "D³uga"
    };

    var user = new User()
    {
        Email = "user@test.com",
        FullName = "Test User",
        Address = address
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();
});

app.MapDelete("delete", async (MyBoardsContext db) =>
{
    var user = await db.Users
    .Include(u => u.Comments)
    .FirstAsync(u => u.Id == Guid.Parse("4EBB526D-2196-41E1-CBDA-08DA10AB0E61"));

    db.Users.Remove(user);

    await db.SaveChangesAsync();
});
#endregion
app.Run();
