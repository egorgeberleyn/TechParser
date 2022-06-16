using Microsoft.EntityFrameworkCore;
using TechParser.Core.Data;
using TechParser.Core.Parser;

namespace TechParser;

public class Program
{
    public static async Task Main()
    {           
        var host = CreateHostBuilder().Build();
        using (var scope = host.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider; //получение провайдера для разрешения зависимостей
            try
            {
                var context = serviceProvider.GetRequiredService<ParserDbContext>(); //получение контекста БД                
                await DbInitializer.Initialize(context);  //инициализация на основе контекста                
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);//логирование фатального уровня
            }
        }       
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()          
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

