using Hydra.DAL.Core;
using Hydra.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraTentacle.Core.DAL
{
    public class TentacleUnitOfWork : UnitOfWork
    {
        public TentacleUnitOfWork(DbContext context, ILogService logService) : base(context, logService)
        {
        }
    }
}
