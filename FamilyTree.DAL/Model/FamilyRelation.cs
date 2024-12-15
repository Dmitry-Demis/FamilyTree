namespace FamilyTree.DAL.Model;

public class FamilyRelation : IEntity
{
    public int? ParentId { get; set; }  // Идентификатор родителя
    public Person Parent { get; set; }  // Родитель (связь с сущностью Person)

    public int? ChildId { get; set; }  // Идентификатор ребёнка
    public Person Child { get; set; }  // Ребёнок (связь с сущностью Person)
    public int Id { get; init; }
}