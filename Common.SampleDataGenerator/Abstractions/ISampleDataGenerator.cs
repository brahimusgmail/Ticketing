namespace Common.SampleDataGenerator.Abstractions;

public interface ISampleDataGenerator
{
    /// <summary>
    /// Retourne un jeu de données d'exemple basé sur le schéma et les instructions fournis.
    /// </summary>
    /// <typeparam name="T">type retourné exemple AITicketSeedDto.</typeparam>
    /// <param name="schema">le schéma json utilisé pour générer les données.</param>
    /// <param name="instruction">les instructions pour générer les données (prompt.</param>
    /// <param name="recordCount">le nombre d'enregistrements à générer.</param>
    /// <param name="cancellationToken">le jeton d'annulation.</param>
    /// <returns>la liste des données d'exemple.</returns>
    Task<IReadOnlyList<T>> GenerateAsync<T>(
    object schema,
    string instruction,
    int recordCount,
    CancellationToken cancellationToken = default);
}
