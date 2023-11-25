using BackingFields.EF8.Simple;

using (var dbContext = new PersonContext())
{
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();

    await dbContext.AddRangeAsync(new List<Person>
        {
            new Person
            {
                Age = 9,
            },
            new Person
            {
                Age = 49,
            }
        });
    await dbContext.SaveChangesAsync();
}

using (var dbContext = new PersonContext())
{
    dbContext.People.Select(p => new
    {
        p.Id,
        p.Age,
        p.IsOver18
    })
    .ToList()
    .ForEach(p => Console.WriteLine($"{p.Age} - {p.IsOver18}"));
}