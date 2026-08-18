namespace MVC_nhaSach.Services;

public class OrderException(string message) : InvalidOperationException(message);
