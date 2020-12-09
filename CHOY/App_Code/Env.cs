namespace CHOY.App_Code
{
    public class Env
    {
        private readonly string _secretKey = "secret key";
        private readonly string _sendEmailAPI = "https://app1118.herokuapp.com/sendEmail";
        public string SecretKey { get { return _secretKey; } }
        public string SendEmailAPI { get { return _sendEmailAPI; } }
    }
}