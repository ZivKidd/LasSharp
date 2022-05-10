using System;
using System.IO;

namespace LasSharp
{
    public class LasReader : IDisposable
    {
        public LasPoint CurrentPoint { get; private set; }

        private int currentPointIndex = 0;

        public bool MoveToNextPoint()
        {
            this.currentPointIndex++;
            if (this.currentPointIndex > this.NumberOfPoints)
            {
                return false;
            }

            double x = this._binaryReader.ReadInt32() * this.XScale + this.XOffset;
            double y = this._binaryReader.ReadInt32() * this.YScale + this.YOffset;
            double z = this._binaryReader.ReadInt32() * this.ZScale + this.ZOffset;
            ushort i = this._binaryReader.ReadUInt16();
            byte ReturnNumber_NumberofReturns_ScanDirectionFlag_EdgeOfFlightLine =
                this._binaryReader.ReadByte();
            byte classification = this._binaryReader.ReadByte();
            byte scanAngleRank = this._binaryReader.ReadByte();
            byte userData = this._binaryReader.ReadByte();
            ushort pointSourceId = this._binaryReader.ReadUInt16();
            double gpsTime = 0;
            switch (this.PointDataFormat)
            {
                case 1:
                    gpsTime = this._binaryReader.ReadDouble();
                    break;
            }

            // this._binaryReader.ReadBytes(this.PointDataRecordLength - 4 * 3 - 2);
            this.CurrentPoint = new LasPoint()
            {
                Intensity = i,
                X = x,
                Y = y,
                Z = z,
                Classification = classification,
                ScanAngleRank = scanAngleRank,
                UserData = userData,
                GPSTime = gpsTime,
                PointSourceID = pointSourceId,
                ReturnNumber_NumberofReturns_ScanDirectionFlag_EdgeOfFlightLine = ReturnNumber_NumberofReturns_ScanDirectionFlag_EdgeOfFlightLine
            };
            return true;
        }

        public void Close()
        {
            this._binaryReader.Close();
        }

        public byte PointDataFormat { get; set; }

        public ushort HeaderSize { get; }

        public ushort FileCreationYear { get; }

        public ushort FileCreationDayOfYear { get; }

        public char[] GeneratingSoftware { get; }

        public char[] GuidData4 { get; }

        public ushort GuidData3 { get; }

        public ushort GuidData2 { get; }

        public uint GuidData1 { get; }

        public uint OffsetToPointData { get; }
        public ushort PointDataRecordLength { get; }
        public uint NumberOfPoints { get; }

        public byte VersionMajor { get; }
        public byte VersionMinor { get; }


        public double XScale { get; }
        public double YScale { get; }
        public double ZScale { get; }

        public double XOffset { get; }
        public double YOffset { get; }
        public double ZOffset { get; }

        public double MaxX { get; }
        public double MinX { get; }
        public double MaxY { get; }
        public double MinY { get; }
        public double MaxZ { get; }
        public double MinZ { get; }

        private BinaryReader _binaryReader;

        public LasReader(string lasFilePath)
        {
            this._binaryReader = new BinaryReader(File.OpenRead(lasFilePath));
            this._binaryReader.ReadBytes(8);//seek to Guid1
            this.GuidData1 = this._binaryReader.ReadUInt32();
            this.GuidData2 = this._binaryReader.ReadUInt16();
            this.GuidData3 = this._binaryReader.ReadUInt16();
            this.GuidData4 = this._binaryReader.ReadChars(8);
            this.VersionMajor = this._binaryReader.ReadByte();
            this.VersionMinor = this._binaryReader.ReadByte();
            var systemIdentifier = this._binaryReader.ReadChars(32);
            this.GeneratingSoftware = this._binaryReader.ReadChars(32);
            this.FileCreationDayOfYear = this._binaryReader.ReadUInt16();
            this.FileCreationYear = this._binaryReader.ReadUInt16();
            this.HeaderSize = this._binaryReader.ReadUInt16();
            this.OffsetToPointData = this._binaryReader.ReadUInt32();
            var numberOfVariable = this._binaryReader.ReadUInt32();
            this.PointDataFormat = this._binaryReader.ReadByte();
            this.PointDataRecordLength = this._binaryReader.ReadUInt16();
            this.NumberOfPoints = this._binaryReader.ReadUInt32();
            var numberOfReturn = this._binaryReader.ReadBytes(20);
            this.XScale = this._binaryReader.ReadDouble();
            this.YScale = this._binaryReader.ReadDouble();
            this.ZScale = this._binaryReader.ReadDouble();
            this.XOffset = this._binaryReader.ReadDouble();
            this.YOffset = this._binaryReader.ReadDouble();
            this.ZOffset = this._binaryReader.ReadDouble();
            this.MaxX = this._binaryReader.ReadDouble();
            this.MinX = this._binaryReader.ReadDouble();
            this.MaxY = this._binaryReader.ReadDouble();
            this.MinY = this._binaryReader.ReadDouble();
            this.MaxZ = this._binaryReader.ReadDouble();
            this.MinZ = this._binaryReader.ReadDouble();

            this._binaryReader.BaseStream.Seek(this.OffsetToPointData, 0);
        }

        public void Dispose()
        {
            this._binaryReader?.Dispose();
        }
    }
}