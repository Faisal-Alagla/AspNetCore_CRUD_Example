using System.ComponentModel.DataAnnotations;

namespace Entities
{
	/// <summary>
	/// Domain Model for Country
	/// </summary>
	public class Country
	{
		[Key]
		public Guid CountryID { get; set; }
		[StringLength(40)]
		public string? CountryName { get; set; }

		//optionally: if we wanna have a list of the persons in every country
		public virtual ICollection<Person>? Persons { get; set; }
	}
}