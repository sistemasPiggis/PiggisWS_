namespace PIGGISWS.Models;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public int Status { get; set; }

}
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class DatabaseException : Exception
{
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}