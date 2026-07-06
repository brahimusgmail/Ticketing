namespace Ticketing.Shared.Enums.Ticket;

using System.ComponentModel.DataAnnotations;

public class TicketEnums
{
    // statut du ticket.
    public enum Status
    {
        /// <summary>
        /// Nouveau.
        /// </summary>
        [Display(Name = "Nouveau")]
        New = 0,

        /// <summary>
        /// En cours.
        /// </summary>
        [Display(Name = "En cours")]
        InProcess = 1,

        /// <summary>
        /// Cloturé.
        /// </summary>
        [Display(Name = "Fermé")]
        Closed = 2,
    }
}
