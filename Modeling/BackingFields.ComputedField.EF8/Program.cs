using BackingFields.ComputedField.EF8;
using Microsoft.EntityFrameworkCore;

await OrderBackingFieldTestingAsync();

async Task PersonBackingFieldTestingAsync()
{
    using (var dbContext = new BackingFields.ComputedField.EF8.AppContext())
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
            },
            //Uncomment to see how validation in setter working with EF Core
            //new Person
            //{
            //    Age = 3,
            //}
        });
        await dbContext.SaveChangesAsync();
    }

    using (var dbContext = new BackingFields.ComputedField.EF8.AppContext())
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
}

async Task OrderBackingFieldTestingAsync()
{
    using (var dbContext = new BackingFields.ComputedField.EF8.AppContext())
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var order = new Order();
        order.SetOrderDate(DateTime.Now.AddDays(1));

        await dbContext.AddAsync(order);
        await dbContext.SaveChangesAsync();
    }

    using (var dbContext = new BackingFields.ComputedField.EF8.AppContext())
    {
        dbContext.Orders.Select(p => new
        {
            p.Id,
            OrderDate = EF.Property<DateTime>(p, "_orderDate")
        })
        .ToList()
        .ForEach(p => Console.WriteLine($"{p.Id} - {p.OrderDate}"));
    }
}
