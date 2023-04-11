﻿using System;
using ServiceContracts.DTO;

namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Person entity
	/// </summary>
	public interface IPersonsService
	{
		/// <summary>
		/// Adds a person object to the list of persons
		/// </summary>
		/// <param name="personAddRequest">Person object to add</param>
		/// <returns>A person object after adding it, along with newly generated PersonID</returns>
		PersonResponse AddPerson(PersonAddRequest? personAddRequest);

		/// <summary>
		/// List of persons
		/// </summary>
		/// <returns>A list of PersonResponse objects</returns>
		List<PersonResponse> GetAllPersons();
	}
}