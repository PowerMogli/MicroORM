RabbitDB
========

A simple abstraction of data access. Simple, feature-rich, clean and hopefully efficent :)

Simple
----------

No dynamics, simple and no obscure command execution. 
If used by convention - no attributes are needed.

Features
------------

Change tracking, identity map, multiple resultsets, custom mappings
(all on request - soon to come).
Seperate `Entity` class to inherit from to use without using within `DbSession`.

```csharp
[Table("Posts")]
    class Post : Entity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public PostType Type { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }
        public string Test1 { get; set; }
        public bool Test2 { get; set; }
    }
```

Clean
-----

Based on clean-code principles and SOLID principles.

Efficient
---------

I don´t know yet.
