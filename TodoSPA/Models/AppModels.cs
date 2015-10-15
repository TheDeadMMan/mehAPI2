
namespace TodoSPA.Controllers
{
    public class DocToken
    {
        public string token { get; set; }
    }
    public class DocOut
    {
        public string name { get; set; }
        public int id { get; set; }
    }
    public class Document : DocToken
    {
        public string name { get; set; }
        public string base64 { get; set; }
    }


}