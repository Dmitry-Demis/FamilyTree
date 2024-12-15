
using FamilyTree.DAL.Model;
using StoreCatalogPresentation.Models;

namespace FamilyTree.Presentation.Models
{
    public class PersonWrapper(Person person) : Wrapper<Person>(person)
    {
        public int Id => Base.Id;

        public string FullName
        {
            get => Base.FullName;
            set => Set(() => Base.FullName, v => Base.FullName = v, value);
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

        public ICollection<FamilyRelation> Parents => Base.Parents;

        public ICollection<FamilyRelation> Children => Base.Children;
    }
}
