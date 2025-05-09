using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Responses
{
    public class ClientReferenceResponse
    {
        [Column("pt_num")]  // Explicit column mapping
        public string PtNum { get; set; }

        [Column("client_reference")]
        public string ClientReference { get; set; }

        [Column("client_index")]
        public string ClientIndex { get; set; }

        [Column("client")]
        public string Client { get; set; }

        [Column("referentiel")]
        public string Referentiel { get; set; }

        [Column("is_serialized")]
        public bool IsSerialized { get; set; }
    }
}
