var construtor = WebApplication.CreateBuilder(args);

construtor.Services.AddControllers();
construtor.Services.AddOpenApi();

var aplicacao = construtor.Build();

if (aplicacao.Environment.IsDevelopment())
{
    aplicacao.MapOpenApi();
}

aplicacao.UseAuthorization();
aplicacao.MapControllers();
aplicacao.Run();
