using System.Net;
namespace Net {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DisconnectReason {
        public DisconnectReasonType DisconnectReasonType { get; set; }
        public IPEndPoint Remote { get; set; }
    }
}
