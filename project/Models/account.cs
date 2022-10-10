namespace project.Models
{
    public class account
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string repeat_password { get; set; }
        public account()
        {

        }
    }
}
