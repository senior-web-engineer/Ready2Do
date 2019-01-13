namespace ready2do.model.common
{
    /// <summary>
    /// Dati di base del profilo di un Utente
    /// Finchè il profilo utente non verrà esteso con informazioni solo locali contiene un sottoinsieme dei dati in B2C
    /// </summary>
    public class UtenteDM: UtenteInputDM
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
