namespace WebApiConsumeDemo.ViewModels;
public class UserViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public UserAddressViewModel? Address { get; set; }
}


public class UserAddressViewModel
{
    public string? Street { get; set; }
    public string? Suite { get; set; }

    public string? Zipcode { get; set; }
    public string? City { get; set; }

    public GeoViewModel? Geo { get; set; }
}

public class GeoViewModel
{
    public string? Lat { get; set; }
    public string? Lng { get; set; }
}
