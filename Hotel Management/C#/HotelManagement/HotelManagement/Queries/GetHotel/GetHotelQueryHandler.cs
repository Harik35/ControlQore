﻿using HotelManagement.Dtos;
using HotelManagement.Model.Model;
using HotelManagement.Repo.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotelManagement.Queries.GetHotel
{
    public class GetHotelQueryHandler : IRequestHandler<GetHotelQuery, List<ViewHotelDto>>
    {
        private readonly HotelManagementContext _context;

        public GetHotelQueryHandler(HotelManagementContext context)
        {
            _context = context;
        }



        public async Task<List<ViewHotelDto>> Handle(GetHotelQuery request, CancellationToken cancellationToken)
        {
            // return await _context.Hotel.Where(x => x.Rating > request.Rating).Select(x => x).ToListAsync();


            /*return await _context.Location.Join(_context.Hotel, x => x.LocationId, y => y.LocationId, (x, y) =>
            new
            {
                Location = x.Name,
                HotelName = y.Name
            }).Select(x => new Location = x.Name,
                HotelName = y.Name).ToListAsync();*/


            //return await _context.Hotel.Where(x => x.Location.Name == request.LocationName &&  x.Rating > request.Rating && request.StartDate > x. )
            //    .Select(x => new ViewHotelDto
            //    {
            //        Id = x.HotelId,
            //        Name = x.Name,
            //        Location  = x.Location.Name,
            //        Rating = x.Rating,
            //    })
            //    .ToListAsync();

            //return await _context.BookedRoom.Where(x => x.Room.Hotel.Location.Name == request.LocationName && x.Room.Hotel.Rating > request.Rating
            //&& (request.StartDate > x.EndDate || request.EndDate < x.StartDate))
            //.Select(x => new ViewHotelDto
            //{
            //    Id = x.Room.Hotel.HotelId,
            //    Name = x.Room.Hotel.Name,
            //    Location = x.Room.Hotel.Location.Name,
            //    Rating = x.Room.Hotel.Rating,
            //    RoomId = x.Room.RoomId
            //}).ToListAsync();


            // return await _context.Room
            //     .Where(x => x.Hotel.LocationId == request.LocationId && x.Hotel.Rating > request.Rating
            //&& !_context.BookedRoom
            //.Any(y => (request.StartDate >= y.StartDate && request.EndDate <= y.EndDate) && x.RoomId == y.RoomId))
            //.Select(x => new ViewHotelDto
            //{
            //    Id = x.Hotel.HotelId,
            //    Name = x.Hotel.Name,
            //    Location = x.Hotel.Location.Name,
            //    Rating = x.Hotel.Rating
            //}).ToListAsync();


            var availRooms = await _context.Room
                .Where(x => x.Hotel.LocationId == request.LocationId && x.Hotel.Rating > request.Rating
           && !_context.BookedRoom
           .Any(y => (request.StartDate >= y.StartDate && request.EndDate <= y.EndDate) && x.RoomId == y.RoomId))
           .Select(x => new ViewHotelDto
           {
               Id = x.Hotel.HotelId,
               Name = x.Hotel.Name,
               Location = x.Hotel.Location.Name,
               Rating = x.Hotel.Rating,
               RoomTypeId = x.RoomTypeId
           }).ToListAsync();

            var selectedHotelRooms= availRooms.GroupBy(x => new { x.RoomTypeId, x.Id })
                .Select(x => new ViewRoomCount
                { Id = x.Key.RoomTypeId,
                  CountofRooms = x.Count()
                }).ToList();

            for (var i = 0; i < selectedHotelRooms.Count; i++)
            {
                if(selectedHotelRooms[i].CountofRooms >= request.NoOfRooms)
                {
                   return await _context.Room
                 .Where(x => x.Hotel.LocationId == request.LocationId && x.RoomTypeId == selectedHotelRooms[i].Id && x.Hotel.Rating > request.Rating
            && !_context.BookedRoom 
            .Any(y => (request.StartDate >= y.StartDate && request.EndDate <= y.EndDate) && x.RoomId == y.RoomId))
            .Select(x => new ViewHotelDto
            {
                Id = x.Hotel.HotelId,
                Name = x.Hotel.Name,
                Location = x.Hotel.Location.Name,
                Rating = x.Hotel.Rating,
                RoomTypeId = x.RoomTypeId
            }).Distinct().ToListAsync();       
                }

                else
                {
                    return await _context.Room.Select(
                        x => new ViewHotelDto
                        {
                            Id =0,
                            Name = "",
                            Location ="",
                            Rating = 0,
                            RoomTypeId = 0
                        }).ToListAsync();
                }
            }

            return await _context.Room.Select(
                        x => new ViewHotelDto
                        {
                            Id = 0,
                            Name = "",
                            Location = "",
                            Rating = 0,
                            RoomTypeId = 0
                        }).ToListAsync();



        }
    }
}
