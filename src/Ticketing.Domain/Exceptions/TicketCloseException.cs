namespace Ticketing.Domain.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class TicketCloseException : DomainException
    {
        public TicketCloseException()
            : base("Vous ne pouver pas ajouter un commentaire à un ticket fermé")
        {
        }
    }
}
