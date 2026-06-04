namespace TimeRecord.DTO.Auth
{
    public class AuthResponseTokenDTO
    {
        public int StatusCode { get; set; }
       
        public string  Message { get; set; }
       
        public bool  Authentication  { get; set; }
    }
}
