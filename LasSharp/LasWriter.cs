using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LasSharp
{
    public class LasWriter : IDisposable
    {
        private readonly double _xScale;
        private readonly double _yScale;
        private readonly double _zScale;
        private readonly double _xOffset;
        private readonly double _yOffset;
        private readonly double _zOffset;
        private readonly byte _versionMajor;
        private readonly byte _versionMinor;
        private readonly byte _pointDataFormat;

        private BinaryWriter _binaryWriter;

        private double _xMin = double.MaxValue;
        private double _yMin = double.MaxValue;
        private double _zMin = double.MaxValue;
        private double _xMax = double.MinValue;
        private double _yMax = double.MinValue;
        private double _zMax = double.MinValue;
        private int _numberofpoints = 0;

        public void WritePoint(LasPoint lasPoint)
        {
            this._numberofpoints++;
            this._xMax = Math.Max(lasPoint.X, this._xMax);
            this._xMin = Math.Min(lasPoint.X, this._xMin);
            this._yMax = Math.Max(lasPoint.Y, this._yMax);
            this._yMin = Math.Min(lasPoint.Y, this._yMin);
            this._zMax = Math.Max(lasPoint.Z, this._zMax);
            this._zMin = Math.Min(lasPoint.Z, this._zMin);

            this._binaryWriter.Write(Convert.ToInt32((lasPoint.X - this._xOffset) / this._xScale));
            this._binaryWriter.Write(Convert.ToInt32((lasPoint.Y - this._yOffset) / this._yScale));
            this._binaryWriter.Write(Convert.ToInt32((lasPoint.Z - this._zOffset) / this._zScale));
            this._binaryWriter.Write((ushort)lasPoint.Intensity);

            this._binaryWriter.Write(lasPoint.ReturnNumber_NumberofReturns_ScanDirectionFlag_EdgeOfFlightLine);
            this._binaryWriter.Write(lasPoint.Classification);
            this._binaryWriter.Write(lasPoint.ScanAngleRank);
            this._binaryWriter.Write(lasPoint.UserData);
            this._binaryWriter.Write(lasPoint.PointSourceID);
            switch (this._pointDataFormat)
            {
                case 1:
                    this._binaryWriter.Write(lasPoint.GPSTime);
                    break;
            }
        }

        public void Close()
        {
            //更新点数，XYZ坐标范围等信息
            FileStream writeStream = (FileStream)this._binaryWriter.BaseStream;
            writeStream.Seek(107, SeekOrigin.Begin);
            writeStream.Write(BitConverter.GetBytes(this._numberofpoints), 0, 4);
            writeStream.Seek(179, SeekOrigin.Begin);
            writeStream.Write(BitConverter.GetBytes(this._xMax), 0, 8);
            writeStream.Write(BitConverter.GetBytes(this._xMin), 0, 8);
            writeStream.Write(BitConverter.GetBytes(this._yMax), 0, 8);
            writeStream.Write(BitConverter.GetBytes(this._yMin), 0, 8);
            writeStream.Write(BitConverter.GetBytes(this._zMax), 0, 8);
            writeStream.Write(BitConverter.GetBytes(this._zMin), 0, 8);
            writeStream.Close();
            this._binaryWriter.Close();
        }

        public LasWriter(string lasFilePath, double xScale = 0.0001, double yScale = 0.0001, double zScale = 0.0001, double xOffset = 0, double yOffset = 0, double zOffset = 0,
            byte versionMajor = 1, byte versionMinor = 1, byte pointDataFormat = 1)
        {
            if (versionMajor != 1)
            {
                throw (new Exception("given VersionMajor is not supported yet"));
            }
            if (versionMinor != 1)
            {
                throw (new Exception("given versionMinor is not supported yet"));
            }

            if (pointDataFormat != 0 && pointDataFormat != 1)
            {
                throw (new Exception("given pointDataFormat is not supported yet"));
            }
            this._xScale = xScale;
            this._yScale = yScale;
            this._zScale = zScale;
            this._xOffset = xOffset;
            this._yOffset = yOffset;
            this._zOffset = zOffset;
            this._versionMajor = versionMajor;
            this._versionMinor = versionMinor;
            this._pointDataFormat = pointDataFormat;

            this._binaryWriter = new BinaryWriter(File.Open(lasFilePath, FileMode.Create, FileAccess.Write), Encoding.ASCII);
            string fileSignature = "LASF";
            this._binaryWriter.Write(Encoding.ASCII.GetBytes(fileSignature));
            ushort fileSourceId = 0;
            this._binaryWriter.Write(fileSourceId);
            ushort reserved = 0;
            this._binaryWriter.Write(reserved);
            uint prjidGuid1 = 0;
            this._binaryWriter.Write(prjidGuid1);
            ushort prjidGuid2 = 0;
            this._binaryWriter.Write(prjidGuid2);
            ushort prjidGuid3 = 0;
            this._binaryWriter.Write(prjidGuid3);
            char[] prjidGuid4 = new char[8];
            this._binaryWriter.Write(prjidGuid4);

            this._binaryWriter.Write(_versionMajor);

            this._binaryWriter.Write(_versionMinor);

            char[] systemIdentifier = new char[32];
            this._binaryWriter.Write(systemIdentifier);

            string generatingSoft = "github.com/ZivKidd/LasSharp    ";
            this._binaryWriter.Write(generatingSoft);

            ushort fileCreationDayOfYear = (ushort)DateTime.Now.DayOfYear;
            this._binaryWriter.Write(fileCreationDayOfYear);

            ushort fileCreationYear = (ushort)DateTime.Now.Year;
            this._binaryWriter.Write(fileCreationYear);

            ushort headerSize = 227;
            this._binaryWriter.Write(headerSize);

            int offsetToPointData = 227;
            this._binaryWriter.Write(offsetToPointData);

            int numberOfVariable = 0;
            this._binaryWriter.Write(numberOfVariable);

            this._binaryWriter.Write(pointDataFormat);

            ushort pointDataRecordLength = 0;
            switch (pointDataFormat)
            {
                case 0:
                    pointDataRecordLength = 20;
                    break;
                case 1:
                    pointDataRecordLength = 28;
                    break;
            }
            this._binaryWriter.Write(pointDataRecordLength);

            this._binaryWriter.Write(this._numberofpoints);
            int return0 = 0;
            int return1 = 0;
            int return2 = 0;
            int return3 = 0;
            int return4 = 0;
            this._binaryWriter.Write(return0);
            this._binaryWriter.Write(return1);
            this._binaryWriter.Write(return2);
            this._binaryWriter.Write(return3);
            this._binaryWriter.Write(return4);

            this._binaryWriter.Write(this._xScale);
            this._binaryWriter.Write(this._yScale);
            this._binaryWriter.Write(this._zScale);
            this._binaryWriter.Write(this._xOffset);
            this._binaryWriter.Write(this._yOffset);
            this._binaryWriter.Write(this._zOffset);

            this._binaryWriter.Write(this._xMax);
            this._binaryWriter.Write(this._xMin);
            this._binaryWriter.Write(this._yMax);
            this._binaryWriter.Write(this._yMin);
            this._binaryWriter.Write(this._zMax);
            this._binaryWriter.Write(this._zMin);
        }

        public void Dispose()
        {
            this.Close();
            this._binaryWriter?.Dispose();
        }
    }
}
