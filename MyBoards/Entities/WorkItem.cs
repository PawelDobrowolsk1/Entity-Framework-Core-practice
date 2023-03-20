namespace MyBoards.Entities;

public abstract class WorkItem
{
    public int Id { get; set; }
    public virtual WorkItemState State { get; set; }
    public int StateId { get; set; }
    public string Area { get; set; }
    public string IterationPath { get; set; }
    public int Priority { get; set; }

    public virtual List<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User Author { get; set; }
    public Guid AuthorId { get; set; }
    public virtual List<Tag> Tags { get; set; }

}
