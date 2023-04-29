using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
	/// <summary>
	/// Person domain model class
	/// </summary>
	public class Person
	{
		[Key]
		public Guid PersonID { get; set; }
		[StringLength(40)]
		public string? PersonName { get; set; }
		[StringLength(50)]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		[StringLength(10)]
		public string? Gender { get; set; }
		public Guid? CountryID { get; set; }
		[StringLength(200)]
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		//check fluent api in DBContext
		public string? TIN { get; set; }

		[ForeignKey("CountryID")] //instead of the table table relations in DBContext (commented)
		public virtual Country? Country { get; set; }
	}
}
