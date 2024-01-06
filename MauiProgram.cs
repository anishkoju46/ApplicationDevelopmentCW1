using CourseWorkAppDev1.Modules.Admin.service;
using CourseWorkAppDev1.Modules.Coffee.service;
using CourseWorkAppDev1.Modules.Staff.service;
using CourseWorkAppDev1.Utils.HelperServices;
using Microsoft.Extensions.Logging;

namespace CourseWorkAppDev1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AdminService>();
            builder.Services.AddSingleton<CoffeeService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<ActionService>();
            builder.Services.AddSingleton<StaffService>();
            return builder.Build();
        }
    }
}
