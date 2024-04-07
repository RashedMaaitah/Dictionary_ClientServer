namespace Client.Exceptions;

/// <summary>
/// Custom Exception class
/// </summary>
/// <param name="msg"></param>
class ResourceNotFoundException(string msg = "Resource not found") : Exception(msg)
{
}
