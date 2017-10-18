namespace eHesabim.Framework.Facebook {
    public interface IFacebookService {
        string GetLoginUrl();

        string GetAccessToken(string code);

        Person GetUserInfo(string accessToken);
    }
}
