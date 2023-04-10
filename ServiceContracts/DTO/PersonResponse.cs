using System;
using Entities;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO class that is used as return type for most of PersonService methods
	/// </summary>
	public class PersonResponse
	{
		public Guid PersonID { get; set; }
		public string? PersonName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public Guid? CountryID { get; set; }
		public string? Country { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }
		public double? Age { get; set; }

		//Compares the current object with another PersonResponse type
		//Returns true if their values are same, false otherwise
		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != typeof(PersonResponse)) return false;

			PersonResponse person_to_compare = (PersonResponse)obj;

			return this.PersonID == person_to_compare.PersonID &&
					this.PersonName == person_to_compare.PersonName &&
					this.Email == person_to_compare.Email &&
					this.DateOfBirth == person_to_compare.DateOfBirth &&
					this.Gender == person_to_compare.Gender &&
					this.CountryID == person_to_compare.CountryID &&
					this.Address == person_to_compare.Address &&
					this.ReceiveNewsLetters == person_to_compare.ReceiveNewsLetters &&
					this.Age == person_to_compare.Age;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public static class PersonExtensions
	{
		/// <summary>
		/// An extension method to convert a Person object into a PersonResponse object
		/// </summary>
		/// <param name="person">Person object to be converted</param>
		/// <returns>A PersonResponse object with the same data as the provided Person object</returns>
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse()
			{
				PersonID = person.PersonID,
				PersonName = person.PersonName,
				Email = person.Email,
				DateOfBirth = person.DateOfBirth,
				Gender = person.Gender,
				CountryID = person.CountryID,
				Address = person.Address,
				ReceiveNewsLetters = person.ReceiveNewsLetters,
				Age = (person.DateOfBirth != null) ?
				Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
			};
		}
	}
}
