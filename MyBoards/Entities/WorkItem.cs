namespace MyBoards.Entities;

public abstract class WorkItem
{
    public int Id { get; set; }
    public WorkItemState State { get; set; }
    public int StateId { get; set; }
    public string Area { get; set; }
    public string IterationPath { get; set; }
    public int Priority { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    public User Author { get; set; }
    public Guid AuthorId { get; set; }
    public List<Tag> Tags { get; set; }

}
