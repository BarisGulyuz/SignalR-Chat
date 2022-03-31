using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection servies = builder.Services;

servies.AddSignalR();
servies.AddCors(opt =>
{
    opt.AddPolicy("ChatHub", policy =>
    {
        policy.AllowCredentials();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.SetIsOriginAllowed(x => true);
    });
});

var app = builder.Build();

app.UseCors("ChatHub");
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
