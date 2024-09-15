﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Invoices.Data.Models;

namespace Invoices.DataProcessor.ImportDto
{
    using static Data.DataConstrains;

    [XmlType(nameof(Address))]
    public class ImportAddressDto
    {
        [XmlElement(nameof(StreetName))]
        [Required]
        [MinLength(AddressStreetNameMinLength)]
        [MaxLength(AddressStreetNameMaxLength)]
        public string StreetName { get; set; } = null!;

        [XmlElement(nameof(StreetNumber))]
        public int StreetNumber { get; set; }

        [XmlElement(nameof(PostCode))]
        [Required]
        public string PostCode { get; set; } = null!;

        [XmlElement(nameof(City))]
        [Required]
        [MinLength(AddressCityMinLength)]
        [MaxLength(AddressCityMaxLength)]
        public string City { get; set; } = null!;

        [XmlElement(nameof(Country))]
        [Required]
        [MinLength(AddressCountryMinLength)]
        [MaxLength(AddressCountryMaxLength)]
        public string Country { get; set; } = null!;
    }
}
