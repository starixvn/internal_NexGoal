namespace NexGoal.ViewModels.Auth
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string errorSummary { get; set; }

        public string ReturnUrl { get; set; }
    }
}
