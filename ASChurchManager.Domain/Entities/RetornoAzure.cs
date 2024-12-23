using System.IO;

namespace ASChurchManager.Domain.Entities
{
    public class RetornoAzure : BaseEntity
    {
        public Stream BlobStream { get; set; }
        public string ContentType { get; set; }
        public string Nome { get; set; }
        public byte[] BlobArray { get; set; }
    }
}
