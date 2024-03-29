﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace SharedLib.Models
{
    [Collection("customers")]
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string id { get; set; }

        public string type { get; set; }
        public string customerId { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string creationDate { get; set; }
        public List<CustomerAddress> addresses { get; set; }
        public Password password { get; set; }
        public int salesOrderCount { get; set; }
        public float[]? vector { get; set; }
    }

    public class Password
    {
        public Password(string hash, string salt)
        {
            this.hash = hash;
            this.salt = salt;
        }

        public string hash { get; set; }
        public string salt { get; set; }
    }

    public class CustomerAddress
    {
        public CustomerAddress(string addressLine1, string addressLine2, string city, string state, string country,
            string zipCode, Location location)
        {
            this.addressLine1 = addressLine1;
            this.addressLine2 = addressLine2;
            this.city = city;
            this.state = state;
            this.country = country;
            this.zipCode = zipCode;
            this.location = location;
        }

        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipCode { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public Location(string type, List<float> coordinates)
        {
            this.type = type;
            this.coordinates = coordinates;
        }

        public string type { get; set; }
        public List<float> coordinates { get; set; }
    }
}