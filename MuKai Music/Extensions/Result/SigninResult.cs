namespace MuKai_Music.Model.Manager
{
    public class SigninResult
    {
        public static SigninResult Success(string token) => new SigninResult { Succeeded = true, Token = token };
        public static SigninResult Failed() => new SigninResult();
        public static SigninResult LockedOut() => new SigninResult { IsLockedOut = true };
        public static SigninResult NotAllowd() => new SigninResult { IsNotAllowed = true };
        public static SigninResult TowFactorRequired() => new SigninResult { RequiresTwoFactor = true };

        public string Token { get; private set; }

        public bool Succeeded { get; private set; }
        public bool IsLockedOut { get; private set; }
        public bool IsNotAllowed { get; private set; }
        public bool RequiresTwoFactor { get; private set; }
    }


}
