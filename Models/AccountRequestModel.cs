namespace BridgeWater.Models
{
    public class AccountRequestModel
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? address { get; set; }
        public string? confirmPassword { get; set; }
        public IFormFile? avatar { get; set; }
        public bool admin { get; set; }
    }
}
