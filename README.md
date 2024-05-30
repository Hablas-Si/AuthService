# AuthService

## Om projektet
AuthService er et ASP.NET Core Web API-projekt, der håndterer godkendelse og genererer JSON Web Tokens (JWT) baseret på brugerlogin. Denne tjeneste er forbundet til HashiCorp Vault til sikker opbevaring og hentning af hemmeligheder såsom forbindelsesstrenge til  JWT Secret og Issuer. Den bruger kald og metoder på UserService igennem HttpClient.

## Funktioner
- **Login Endpoint**: Der er 2 login endpoints. Hver af disse tjekker brugerens rolle i databasen og tillader kaldet udfra dette. Brugere logger ind og genererer en JWT token og en Role Claim i den med passende rolle baseret på brugeroplysninger.
- **Authorize Sikkerhed på endepunkter**: Sikrer adgang til beskyttede ressourcer ved hjælp af JWT-tokenvalidering.
- **Integration med HashiCorp Vault**: Henter følsomme oplysninger som autentiseringsnøgler fra Vault, connection strenge til mongodb og redis.
- **JWT-generering med roller**: Genererer JWT med to typer roller: "Admin" og "User", baseret på brugerens adgangsniveau fra UserService databasen. Denne role claim giver adgang til forskellige endepunkter igennem alle services.

## Integration med "UserService" Web API
- Kommunikation mellem disse tjenester kræver ikke direkte databehandling. AuthService generer JWT udfra en post login metode, som sender en request i UserService som validerer om der findes en med samme brugernavn og password og rolle som passer med indtastet body. Herefter vil AuthService generer JWT med rolle som en claim som kan bruges ud i de andre services.

## Installation og kørsel til development
1. Sørg for at have .NET Core SDK installeret på din maskine.
2. Klone dette repository til din lokale maskine.
3. auth-test-env indeholder en samlet docker-compose.yml fil kør denne for at viderudvikle.
4. Test endepunkterne ved hjælp af et værktøj som f.eks. Postman.
