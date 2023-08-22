namespace SWITSTIGPTY.Services;

public class AutoDeleteGames : IHostedService
{
    private Timer _loopTimer;
    private readonly GameHubService _gameHubService;
    private readonly GameService _gameService;

    public AutoDeleteGames(GameHubService gameHubService, GameService gameService)
    {
        _gameHubService = gameHubService;
        _gameService = gameService;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // lancer le timer a 8h du matin
        var now = DateTime.Now;
        var next8am = now.Date.AddDays(1).AddHours(8);
        var delay = next8am - now;
        _loopTimer = new Timer(LoopAsync, null, delay, TimeSpan.FromDays(1));

    }

    private void LoopAsync(object? state)
    {
        // supprimer toutes les games
        var games = _gameService.GetGames();
        foreach (var game in games)
        {
            _gameService.EndGame(game.GameCode);
            _gameHubService.NotifyGameEnded(game.GameCode);
        }
        
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _loopTimer?.Change(Timeout.Infinite, 0);
    }
}