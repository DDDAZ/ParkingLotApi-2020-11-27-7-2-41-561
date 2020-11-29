﻿using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dtos;
using ParkingLotApi.Entities;
using ParkingLotApi.Models;
using ParkingLotApi.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Services
{
    public class ParkingLotService
    {
        private readonly ParkingLotContext context;

        public ParkingLotService(ParkingLotContext context)
        {
            this.context = context;
        }

        public async Task<string> AddParkingLot(ParkingLot parkingLot)
        {
            var parkingLotEntity = await context.ParkingLots.AddAsync(new ParkingLotEntity(parkingLot));
            await context.SaveChangesAsync();

            return parkingLotEntity.Entity.Name;
        }

        public async Task<ParkingLot> GetParkingLotByName(string name)
        {
            var parkingLotEntityFound = await context.ParkingLots.FirstOrDefaultAsync(lot => lot.Name == name);

            return parkingLotEntityFound == null ? null : new ParkingLot(parkingLotEntityFound);
        }

        public async Task DeleteParkingLot(string name)
        {
            var parkingLotEntity = context.ParkingLots.FirstOrDefaultAsync(lot => lot.Name == name).Result;
            context.ParkingLots.Remove(parkingLotEntity);
            await context.SaveChangesAsync();
        }

        public async Task<List<ParkingLot>> GetAllParkingLots()
        {
            var parkingLotEntities = await context.ParkingLots.ToListAsync();

            return parkingLotEntities.Select(lot => new ParkingLot(lot)).ToList();
        }

        public async Task<List<ParkingLot>> GetAllParkingLots(int page)
        {
            const int chunkSize = 15;
            var parkingLotEntities = await context.ParkingLots.ToListAsync();
            var lotLists = parkingLotEntities.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            return lotLists[page - 1].Select(lot => new ParkingLot(lot)).ToList();
        }

        public async Task UpdateParkingLot(string name, ParkingLotUpdateModel data)
        {
            var parkingLotEntity = context.ParkingLots.FirstOrDefaultAsync(lot => lot.Name == name).Result;
            parkingLotEntity.Capacity = data.Capacity;
            await context.SaveChangesAsync();
        }
    }
}
