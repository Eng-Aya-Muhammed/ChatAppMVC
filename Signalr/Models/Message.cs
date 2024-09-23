using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalR.Models
{
    [Table("chatmessage")]
    public class Message
    {
        [Key]
        public int id { get; set; }

        public string messagetxt { get; set; }

        [StringLength(50)]
        public string username { get; set; }
        [StringLength(50)]
        public string? groupname { get; set; }

    }
}

