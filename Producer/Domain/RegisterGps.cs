using System;

namespace Producer.Domain
{
    public class RegisterGps
    {
        public string idTrackingInfoGPS { get; set; }
        public string licensePlate { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public DateTime dateEventGPS { get; set; }
        public DateTime dateEventAVL { get; set; }
        public string typeEvent { get; set; }
        public string data { get; set; }
    }
}