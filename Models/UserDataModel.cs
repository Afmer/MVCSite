namespace MVCSite.Models;
public class UserDataModel
{
    public int Id {get; set;}
    public string Name {get; set;}
    public string Login {get; set;}
    public string Password {get; set;}
    public UserDataModel(string name, string login, string password)
    {
        Name = name;
        Login = login;
        Password = password;
    }
}