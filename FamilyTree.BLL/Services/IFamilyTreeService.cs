using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.DAL.Model;

namespace FamilyTree.BLL.Services
{
    public interface IFamilyTreeService
    {
        Task<bool> CreatePersonAsync(Person person);
        Task<IEnumerable<Person>> LoadFamilyTreeAsync();
    }
}
