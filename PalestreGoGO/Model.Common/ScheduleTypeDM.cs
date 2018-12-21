namespace ready2do.model.common
{
    public enum ScheduleTypeDM: short
    {
        APagamentoRiservatoAbbonati   = 1,   
        GratuitoRiservatoAbbonati     = 10,
        GratuitoRiservatoNonAbbonati  = 11,
        GratuitoDisponibilePerTutti   = 20,
        
        //APagamentoPerTutti => Al momento non riusciamo a gestirlo, dobbiamo prevedere un meccanismo per il pagamento on line o gestire il pagamento "onsite" (ha senso?)
        //GratuitoAbbonatiPagamentoNonAbbonati => come sopra
    }

}
