using System;

namespace ready2do.model.common
{
    [Flags]
    public enum GiorniSettimanaDM
    {
        Nessuno = 0,
        Lunedi = 1 << 1,
        Martedi = 1 << 2,
        Mercoledi = 1 << 3,
        Giovedi = 1 << 4,
        Venerdi = 1 << 5,
        Sabato = 1 << 6,
        Domenica = 1 << 7
    }
}
