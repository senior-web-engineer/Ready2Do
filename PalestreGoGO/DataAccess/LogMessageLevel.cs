namespace PalestreGoGo.DataAccess
{
    public enum LogMessageLevel: byte
    {
        Unknown = 0,
        Error = (byte)'E',
        Warning = (byte)'W',
        Info = (byte)'I',
        Verbose = (byte)'V'
    }
}
