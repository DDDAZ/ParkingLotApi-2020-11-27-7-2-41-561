﻿using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dtos;
using ParkingLotApi.Repository;
using System.Threading.Tasks;
using ParkingLotApi.Entities;

namespace ParkingLotApi.Services
{
    public class ParkingLotService
    {
        private readonly ParkingLotContext context;

        public ParkingLotService(ParkingLotContext context)
        {
            this.context = context;
        }

        public async Task<int> AddParkingLot(ParkingLot parkingLot)
        {
            var parkingLotEntity = await context.ParkingLots.AddAsync(new ParkingLotEntity(parkingLot));
            await context.SaveChangesAsync();

            return parkingLotEntity.Entity.Id;
        }

        public async Task<ParkingLot> GetParkingLotById(int id)
        {
            var parkingLotEntityFound = await context.ParkingLots.FirstOrDefaultAsync(lot => lot.Id == id);

            return parkingLotEntityFound == null ? null : new ParkingLot(parkingLotEntityFound);
        }
    }
}