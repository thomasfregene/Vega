using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vega.Core;
using Vega.Core.Models;
using Vega.Extensions;

namespace Vega.Persistence
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VegaDbContext _context;

        public VehicleRepository(VegaDbContext context)
        {
            _context = context;
        }
        //repository is used to handle query with multiple usage
        public async Task<Vehicle> GetVehicle(int id, bool includeRelated = true)
        {
            if (!includeRelated)
                return await _context.Vehicles.FindAsync(id);
            
            return await _context.Vehicles.Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .SingleOrDefaultAsync(v => v.Id == id);
        }

        public void Add(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }

        public async Task<QueryResult<Vehicle>> GetVehicles(VehicleQuery queryObj)
        {
            var result = new QueryResult<Vehicle>();

            var query = _context.Vehicles
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                
                .AsQueryable();

            query.ApplyFiltering(queryObj);


                //dictionary is used for mapping keys and values
                Dictionary<string, Expression<Func<Vehicle, object>>> columnsMap =
                    new Dictionary<string, Expression<Func<Vehicle, object>>>()
                    {
                        ["makes"] = v=>v.Model.Make.Name,
                        ["model"] = v=>v.Model.Name,
                        ["contactName"] = v => v.ContactName,
                    };
                query = query.ApplyOrdering(queryObj, columnsMap);

                result.TotalItems = await query.CountAsync();
                //logic for pagination
                query = query.ApplyPaging(queryObj);
           
            result.Items = await query.ToListAsync();

            return result;
        }

       
    }
}
