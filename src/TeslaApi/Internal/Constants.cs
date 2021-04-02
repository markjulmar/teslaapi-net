namespace Julmar.TeslaApi.Internal
{
    internal static class Constants
    {
        public const string TeslaAuth = "https://auth.tesla.com/";
        public const string AuthUrl = TeslaAuth + "oauth2/v3/authorize";
        public const string RedirectUri = TeslaAuth + "void/callback";
        public const string MfaTypesUrl = TeslaAuth + "oauth2/v3/authorize/mfa/factors";
        public const string MfaVerify = TeslaAuth + "oauth2/v3/authorize/mfa/verify";
        public const string TokenExchangeUrl = TeslaAuth + "oauth2/v3/token";

        public const string OwnerEndpoint = "https://owner-api.teslamotors.com/";
        public const string OwnerApiEndpoint = OwnerEndpoint + "api/";
        public const string OwnerApiTokenUrl = OwnerEndpoint + "oauth/token";
        public const string VehiclesApi = OwnerApiEndpoint + "1/vehicles/";

        public const string TESLA_CLIENT_ID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";
        public const string TESLA_CLIENT_SECRET = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";
    }
}
