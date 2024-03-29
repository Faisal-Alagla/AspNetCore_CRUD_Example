﻿using System;
using Entities;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO class that is used as return type for most of CountryService methods
	/// </summary>
	public class CountryResponse
	{
		public Guid CountryID { get; set; }
		public string? CountryName { get; set; }

		//Compares the current object with another CountryResponse type
		//Returns true if their values are same, false otherwise
		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != typeof(CountryResponse)) return false;

			CountryResponse country_to_compare = (CountryResponse)obj;

			return CountryName == country_to_compare.CountryName && CountryID == country_to_compare.CountryID;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public static class CountryExtensions
	{
		/// <summary>
		/// An extension method to convert a Country object into a CountryResponse object
		/// </summary>
		/// <param name="country">Country object to be converted</param>
		/// <returns>A CountrynResponse object with the same data as the provided Country object</returns>
		public static CountryResponse ToCountryResponse(this Country country)
		{
			return new CountryResponse()
			{
				CountryID = country.CountryID,
				CountryName = country.CountryName,
			};
		}
	}
}
