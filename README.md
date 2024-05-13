AuthService

Om projektet
AuthService er et ASP.NET Core Web API-projekt, der håndterer godkendelse og genererer JSON Web Tokens (JWT) baseret på brugerlogin. Denne tjeneste er forbundet til HashiCorp Vault til sikker opbevaring og hentning af hemmeligheder såsom forbindelsesstrenge til MongoDB, JWT Secret og Issuer.

Funktioner
- Login Endpoint: Tillader brugere at logge ind og genererer en JWT med passende roller baseret på brugeroplysninger.
- Authorize Endpoint: Sikrer adgang til beskyttede ressourcer ved hjælp af JWT-tokenvalidering.
- Integration med HashiCorp Vault: Henter følsomme oplysninger som forbindelsesstrenge og autentiseringsnøgler fra Vault.
- JWT-generering med roller: Genererer JWT med to typer roller: "admin" og "user", baseret på brugerens adgangsniveau.

Projektstruktur
Projektstrukturen kan se sådan ud:

AuthService/
│
├── Controllers/
│   ├── AuthManagerController.cs
│   └── ...
│
├── Repositories/
│   ├── IMongoDBRepository.cs
│   ├── MongoDBLoginRepository.cs
│   ├── IVaultService.cs
│   ├── VaultService.cs
│   └── ...
│
├── Models/
│   ├── LoginModel.cs
│   └── ...
│
├── appsettings.json
├── Program.cs
├── Startup.cs
└── ...

Konfiguration
- Konfigurationsparametre, såsom Vault-adresse og autentifikationsnøgler, skal angives i appsettings.json-filen.
- Sørg for at opdatere miljøvariabler eller konfigurationsfiler til at indeholde de nødvendige oplysninger for integration med MongoDB og andre tjenester.

Integration med "User" Web API
- AuthService arbejder sammen med en anden Web API, der genererer JWT baseret på roller.
- Kommunikation mellem disse tjenester kræver ikke direkte databehandling. AuthService sender JWT til User Web API, og User Web API kan bruge disse tokens til at give eller nægte adgang til ressourcer baseret på roller.

Installation og kørsel
1. Sørg for at have .NET Core SDK installeret på din maskine.
2. Klone dette repository til din lokale maskine.
3. Konfigurer Vault-adresse og autentifikationsnøgler i appsettings.json.
4. Åbn en terminal, naviger til projektets rodmappe og kør dotnet run.
5. Test endepunkterne ved hjælp af et værktøj som f.eks. Postman.

Bidrag
Bidrag til projektet er velkomne! Du kan åbne en issue for at rapportere fejl eller anmode om nye funktioner. Du kan også oprette en pull-anmodning med forbedringer eller rettelser.
