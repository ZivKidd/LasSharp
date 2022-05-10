namespace LasSharp
{
    public struct LasPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public ushort Intensity { get; set; }
        public byte ReturnNumber_NumberofReturns_ScanDirectionFlag_EdgeOfFlightLine { get; set; }
        public byte Classification { get; set; }
        public byte ScanAngleRank { get; set; }
        public byte UserData { get; set; }
        public ushort PointSourceID { get; set; }
        public double GPSTime { get; set; }
    }
}