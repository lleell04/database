namespace BackendCode.DTOs.LoginModel
{
    public class LoginModel
    {
        //包含登录信息（如用户名和密码）的模型
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

}
