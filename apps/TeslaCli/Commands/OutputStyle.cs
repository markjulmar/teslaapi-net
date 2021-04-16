using System;

namespace TeslaCli.Commands
{
    [Flags]
    public enum OutputStyle
    {
        //Default = 0,
        Text = 1,
        Csv = 2,
        Table = 4,
        JSON = 8,
        //All = Text | Csv | Table | JSON,
        //NoTable = Text | Csv | JSON
    }
}
