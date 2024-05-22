# Gamle værdier til JWT som bliver taget fra vault nu. Har dem herinde hvis de skal indsættes i vault igen. Husk at kalde mount "hemmeligheder" (se get metoden i VaultService.cs hvis du er i tvivl hvorfor).

export Secret=7Y6v8P0QrcdPlrV9UfY6+bMTjx5u8zPC
export Issuer=MinAuthService

# Til adgang til vault. Tjek program.cs hvis du er i tvivl om hvordan det fungerer

export Address=https://localhost:8201/
export Token=00000000-0000-0000-0000-000000000000

# OBS HUSK AT INDSÆTTE export i UserService til mongodb connection
export UserServiceUrl=forkert
