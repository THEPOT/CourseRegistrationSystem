using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseRegistration_Domain.Paginate
{
	public class PaginateMapper : Profile
	{
		public PaginateMapper()
		{
			CreateMap(typeof(Paginate<>), typeof(IPaginate<>))
				.ConvertUsing(typeof(PaginateConverter<,>));
		}

	}
}
