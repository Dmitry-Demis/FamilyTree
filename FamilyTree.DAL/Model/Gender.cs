using System.ComponentModel.DataAnnotations;

namespace FamilyTree.DAL.Model;

public enum Gender
{
    [Display(Name = "Мужской")]
    Male,
    [Display(Name = "Женский")]
    Female
}