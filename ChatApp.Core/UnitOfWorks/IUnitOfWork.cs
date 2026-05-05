using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task CommitAsync(); // Commit bat dong bo
        void Commit(); // Commit dong bo
    }
}
