using PIGGISWS.Interfaces;

namespace PIGGISWS.Services.Utils
{
    public class FcmMessageBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // IServiceProvider crea un ámbito
        private readonly ILogger<FcmMessageBackgroundService> _logger;
        

        public FcmMessageBackgroundService(IServiceProvider serviceProvider, ILogger<FcmMessageBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var marcacionesTask = ExecuteMarcacionesAsync(stoppingToken);
            var notificationsTask = ExecuteNotificationsAsync(stoppingToken);

            //await Task.WhenAll(marcacionesTask, notificationsTask);


            await Task.WhenAll(notificationsTask);
        }
        private async Task ExecuteMarcacionesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope()) // Crear un nuevo ámbito
                {
                    var firebaseService = scope.ServiceProvider.GetRequiredService<FireBaseService>();
                    try
                    {
                        //var marcaciones = 
                            await firebaseService.GetMarcacionesAsync();
                        _logger.LogInformation("Se ha ejecutado GetMarcacionesAsync.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al ejecutar GetMarcacionesAsync.");
                    }
                }


                // Espera 30 horas (108000000 ms)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ExecuteNotificationsAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope()) // Crear un nuevo ámbito
                {
                    var firebaseNotificationService = scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
                    try
                    {
                        // Llama a tu método que envía el mensaje FCM
                        await firebaseNotificationService.SendALLFcmMessageAsync();

                        _logger.LogInformation("Se ha ejecutado SendALLFcmMessageAsync.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al ejecutar SendALLFcmMessageAsync.");
                    }
                }

                // Espera 10 minutos (600000 ms)
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
