using System;
using System.ComponentModel.DataAnnotations; // Para o atributo [Key]

namespace LojaDoManoel.API.Models
{
    public class PackingJobLog
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime RequestTimestamp { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
        public bool Success { get; set; }
    }
}