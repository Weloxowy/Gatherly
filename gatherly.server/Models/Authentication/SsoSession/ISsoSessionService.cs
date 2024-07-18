namespace gatherly.server.Models.Authentication.SsoSession;

public interface ISsoSessionService
{
    SsoSession CreateSso(Guid userId, string email); //tworzenie nowego kodu dla zarejestrowanych
    SsoSession CreateSso(string email); //tworzenie nowego kodu dla niezarejestrowanych
    bool ValidSso(Guid userId, string code); //sprawdzanie kodu dla zarejestrowanych
    bool ValidSso(string email, string code); //sprawdzanie kodu dla niezarejestrowanych
    bool IsTokenAlive(Guid sessionId); //czy token istnieje
    SsoSession SsoDetails(string sessionId); //sprawdzenie szczegółów wygenerowanego kodu
    void ExpireSso(Guid sessionId); //deaktywacja kodu (w przypadku wysłania kilku kodów)
}