//ესენი ეიაიმ ჩამანაცვლებია რო ვუშვებდი swagger ზე არ გადავყვადი .net 10 ის გამო

var builder = WebApplication.CreateBuilder(args);

// ამატებს Controller-ების მხარდაჭერას.
builder.Services.AddControllers();

// Swagger-ისთვის აგროვებს ინფორმაციას API-ის მისამართებზე.
builder.Services.AddEndpointsApiExplorer();

// ამზადებს API-ის დოკუმენტაციას.
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // ხელმისაწვდომს ხდის Swagger-ის დოკუმენტაციას.
    app.UseSwagger();

    // რთავს Swagger-ის ვიზუალურ გვერდს.
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// აკავშირებს Controller-ებში აღწერილ მისამართებს აპლიკაციასთან.
app.MapControllers();

app.Run();