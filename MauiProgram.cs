using Microsoft.Extensions.Logging;
using SQLMaui.Data;
using SQLMaui.ViewModels;

namespace SQLMaui
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif      
            /*
             * In this case, the AddSingleton method is used to register the DatabaseContext as a singleton service.
             * A singleton service means that only one instance of DatabaseContext will be created and shared throughout the application.
             * This can be useful when you want to share the same instance of a service across multiple components or when you want to maintain state across different parts of your application.
             * By registering DatabaseContext as a singleton service, it becomes available for other parts of the application to use. 
             * They can simply request an instance of DatabaseContext from the dependency injection container, and the container will provide the same instance that was registered.
            */
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<ProductViewModel>();
            builder.Services.AddSingleton<MainPage>();


            return builder.Build();
        }
    }
}
