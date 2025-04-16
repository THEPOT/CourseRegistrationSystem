using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDQTSystem_Domain.Paginate
{
	public static class PaginateExtension
	{
		public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> queryable, int page, int size, int firstPage = 1)
		{
			if (page < firstPage) page = firstPage;
			var total = await queryable.CountAsync();
			var items = await queryable.Skip((page - firstPage) * size).Take(size).ToListAsync();
			var totalPages = (int)Math.Ceiling(total / (double)size);
			return new Paginate<T>
			{
				Page = page,
				Size = size,
				Total = total,
				Items = items,
				TotalPages = totalPages
			};
		}
	}
}
