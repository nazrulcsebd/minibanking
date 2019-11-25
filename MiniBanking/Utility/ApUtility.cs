using AutoMapper;
using MiniBanking.Core.Helper.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CodeBonds.Utility
{
    public static class ApUtility
    {

        public static Notification CreateNotification(string message, Enum type)
        {
            return new Notification()
            {
                Type = type.ToString(),
                Message = message
            };
        }

        public static TDestination ObjectMapToOther<TSource, TDestination>(TSource sourceObject)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
            var mapper = config.CreateMapper();

            return mapper.Map<TDestination>(sourceObject);
        }

    }
}
