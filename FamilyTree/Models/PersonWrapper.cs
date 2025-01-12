
using FamilyTree.DAL.Model;

namespace FamilyTree.Presentation.Models
{
    public class PersonWrapper(Person person) : Wrapper<Person>(person)
    {
        public int Id => Base.Id;

        public string Name
        {
            get => Base.Name;
            set => Set(() => Base.Name, v => Base.Name = v, value);
        }

        public DateTime DateOfBirth
        {
            get => Base.DateOfBirth;
            set => Set(() => Base.DateOfBirth, v => Base.DateOfBirth = v, value);
        }

        public Gender Gender
        {
            get => Base.Gender;
            set => Set(() => Base.Gender, v => Base.Gender = v, value);
        }

        public int? SpouseId
        {
            get => Base.SpouseId;
            set => Set(() => Base.SpouseId, v => Base.SpouseId = v, value);
        }

        public IEnumerable<FamilyRelation>? Parents => Base.Parents;

        public IEnumerable<FamilyRelation>? Children => Base.Children;
    }
}
