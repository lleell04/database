namespace BackendCode.Models
{
    public class POST_IMAGE
    {
        public string POST_ID { get; set; }
        public string IMAGE_ID { get; set; }
        public byte[] IMAGE { get; set; }
        public virtual POST POST { get; set; }

    }
}
