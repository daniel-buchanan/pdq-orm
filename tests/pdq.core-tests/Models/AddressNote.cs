﻿using System;
using pdq.services;

namespace pdq.core_tests.Models
{
	public class AddressNote
		: Entity<int, int, int>
	{
        public AddressNote()
			: base(nameof(Id), nameof(PersonId), nameof(AddressId))
        {

        }

		public int Id { get; set; }

		public int PersonId { get; set; }

		public int AddressId { get; set; }

		public string Value { get; set; }
	}
}

