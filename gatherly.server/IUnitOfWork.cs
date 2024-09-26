public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Rozpoczyna nową transakcję.
    /// </summary>
    void BeginTransaction();
    
    /// <summary>
    /// Zatwierdza bieżącą transakcję.
    /// </summary>
    void Commit();

    /// <summary>
    /// Wycofuje bieżącą transakcję.
    /// </summary>
    void Rollback();
}