using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    /// <summary>
    /// Non tutte le combinazioni di stati sono valide, 
    /// la coerenza è demndata al codice applicativo
    /// </summary>
    [Flags]
    public enum ClienteUtenteStato: int
    {
        Unknown             =       0,

        NessunAbbonamento   = 1 <<  0,
        AbbonamentoValido   = 1 <<  1,
        AbbonamenoScaduto   = 1 <<  2,

        CertificatoNonPresentato = 1 << 6,
        CertificatoValido = 1 << 7,
        CertificatoScaduto = 1 <<  8,

        NessunPagamentoDovuto= 1 << 10,
        PagamentoDovuto = 1 << 11
    }
}
