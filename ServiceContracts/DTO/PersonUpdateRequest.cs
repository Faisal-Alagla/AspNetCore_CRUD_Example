using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO class for updating an existing person
	/// </summary>
	public class PersonUpdateRequest
	{
		[Required(ErrorMessage = "PersonID can't be blank")]
		public Guid PersonID { get; set; }
		[Required(ErrorMessage = "Person Name can't be blank")]
		public string? PersonName { get; set; }
		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Invalid Email")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		[Required(ErrorMessage = "A gender must be selected")]
		public GenderOptions? Gender { get; set; }
		[Required(ErrorMessage = "A country must be selected")]
		public Guid? CountryID { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		/// <summary>
		/// Converts the current object of PersonUpdateRequest into a new Person object
		/// </summary>
		/// <returns>A Person object that has the same data as the current PersonUpdateRequest object</returns>
		public Person ToPerson()
		{
			return new Person()
			{
				PersonID = PersonID,
				PersonName = PersonName,
				Email = Email,
				DateOfBirth = DateOfBirth,
				Gender = Gender.ToString(),
				CountryID = CountryID,
				Address = Address,
				ReceiveNewsLetters = ReceiveNewsLetters
			};
		}
	}
}
