namespace MVC_nhaSach.Services;

public class CartException(string message) : InvalidOperationException(message);
