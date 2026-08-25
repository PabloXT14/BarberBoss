namespace BarberBoss.Exception.ExceptionsBase;

public abstract class BarberBossException : System.Exception
{
    public abstract int StatusCode { get; }
    public abstract List<string> GetErrors();

    public BarberBossException(string message) : base(message)
    {
    }
}