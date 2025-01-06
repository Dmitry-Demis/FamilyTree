namespace FamilyTree.DAL.Model;

public class FamilyRelation : IEntity
{
    public int Id { get; init; }
    public int? ParentId { get; init; }  // Идентификатор родителя
    public Person? Parent { get; init; }  // Родитель (связь с сущностью Person)

    public int? ChildId { get; init; }  // Идентификатор ребёнка
    public Person? Child { get; init; }  // Ребёнок (связь с сущностью Person)
}