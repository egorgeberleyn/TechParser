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
            var serviceProvider = scope.ServiceProvider; //��������� ���������� ��� ���������� ������������
            try
            {
                var context = serviceProvider.GetRequiredService<ParserDbContext>(); //��������� ��������� ��                
                await DbInitializer.Initialize(context);  //������������� �� ������ ���������                
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);//����������� ���������� ������
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

