﻿using FamilyTree.DAL.Model;

namespace FamilyTree.BLL.Services
{
    public interface IFamilyTreeService
    {
        Task<bool> AddPersonToTreeAsync(Person person);
        Task<IEnumerable<Person>> LoadPeopleAsync();
        Task<bool> AddParentChildRelationAsync(Person parent, Person child);

        Task<bool> AddSpouseRelationAsync(Person person1, Person person2);

        Task<IEnumerable<Person>> GetAllDescendantsAsync(Person p);

        Task<IEnumerable<Person>> GetAllAncestorsAsync(Person p);
    }
}
