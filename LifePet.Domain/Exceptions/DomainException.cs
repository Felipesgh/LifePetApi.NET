namespace LifePet.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

public class BusinessException : DomainException
{
    public BusinessException(string message) : base(message) { }
}
