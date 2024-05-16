# AuthService

## Om projektet
AuthService er et ASP.NET Core Web API-projekt, der håndterer godkendelse og genererer JSON Web Tokens (JWT) baseret på brugerlogin. Denne tjeneste er forbundet til HashiCorp Vault til sikker opbevaring og hentning af hemmeligheder såsom forbindelsesstrenge til MongoDB, JWT Secret og Issuer. Den bruger kald og metoder på UserService igennem HttpClient.

## Funktioner
- **Login Endpoint**: Tillader brugere at logge ind og genererer en JWT med passende roller baseret på brugeroplysninger.
- **Authorize Endpoint**: Sikrer adgang til beskyttede ressourcer ved hjælp af JWT-tokenvalidering.
- **Integration med HashiCorp Vault**: Henter følsomme oplysninger som forbindelsesstrenge og autentiseringsnøgler fra Vault.
- **JWT-generering med roller**: Genererer JWT med to typer roller: "admin" og "user", baseret på brugerens adgangsniveau fra UserService.

## Konfiguration
- Miljøvariablerne er i miljø.md. Dog er det eneste der skal tastes ind i terminal er Address og Token til Vault.

## Integration med "User" Web API
- AuthService arbejder sammen med en anden Web API, der genererer JWT baseret på roller, som hedder UserService.
- Kommunikation mellem disse tjenester kræver ikke direkte databehandling. AuthService generer JWT udfra en post login metode, som sender en request i UserService som validerer om der findes en med samme brugernavn og password som passer med indtastet body. Herefter vil AuthService generer JWT med rolle som en claim som kan bruges ud i de andre services.

## Installation og kørsel
1. Sørg for at have .NET Core SDK installeret på din maskine.
2. Klone dette repository til din lokale maskine.
3. Konfigurer Vault-adresse og Vault-Token fra miljø.md i en terminal.
4. I terminalen, naviger til projektets rodmappe og kør `dotnet run`.
5. Test endepunkterne ved hjælp af et værktøj som f.eks. Postman.
