using System.ComponentModel.DataAnnotations;

namespace Domain.hospital.types;

public class Location
{
    [Required]
    [MinLength(3, ErrorMessage = "Street name must be at least 3 characters long.")]
    [MaxLength(120, ErrorMessage = "Street name cannot exceed 120 characters.")]
    public string StreetName { get; set; }
    [Required]
    [Range(1,9999, ErrorMessage = "Street number must be between 1 and 9999.")]
    public int StreetNumber { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "City must be at least 3 characters long.")]
    [MaxLength(40, ErrorMessage = "City cannot exceed 40 characters.")]
    public string City { get; set; }
    [Required]
    [Range(1,9999, ErrorMessage = "postalcode must be between 1 and 9999.")]
    public string PostalCode { get; set; }
    public string Country { get; set; }
    
    public Location(string streetName, int streetNumber, string city, string postalCode, string country)
    {
        StreetName = streetName;
        StreetNumber = streetNumber;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public Location() {}
}