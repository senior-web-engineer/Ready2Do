using System.ComponentModel.DataAnnotations;

namespace ready2do.model.common
{
    public class TipologiaNotificaDM
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        public bool UserDismissable { get; set; }
        public long? AutoDismisAfter { get; set; }
        public int Priority { get; set; }
    }
}
